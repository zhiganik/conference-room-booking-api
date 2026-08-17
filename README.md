# Conference Room Booking API

A simple, secure API for managing conference rooms, bookings, and rental
pricing — built with ASP.NET Core (.NET 10), EF Core, and SQL Server.

## Business task

A company rents out conference rooms to businesses. Clients need to be
able to:

1. Search which rooms are free for a given date/time window and capacity.
2. Book a room, optionally with extra services (projector, Wi-Fi, sound…).
3. Get the total rental cost calculated automatically, based on *when*
   the room is booked and *which* services are selected.

The cost of the room itself depends on the time of day the booking falls
in, on top of the room's base hourly rate:

| Time band            | Rule                    |
|-----------------------|--------------------------|
| 06:00 – 09:00 (morning)| **-10%** discount        |
| 09:00 – 18:00 (standard, excl. peak) | base rate     |
| 12:00 – 14:00 (peak)  | **+15%** surcharge       |
| 18:00 – 23:00 (evening)| **-20%** discount        |

Bookings that span multiple bands are prorated per minute against each
band and summed (`RentalPriceCalculator`), so a booking crossing e.g.
11:30–12:30 is billed partly at the standard rate and partly at the peak
rate. Selected services are billed at a flat price and added on top of
the time-adjusted room cost.

Admins/managers also need day-to-day business reports — see
[Reports & analytics](#reports--analytics).

## Tech stack

- **.NET 10 / ASP.NET Core Web API**
- **Entity Framework Core 10** + **SQL Server**
- **ASP.NET Core Identity** + **JWT bearer authentication**, role-based
  authorization policies
- **FluentValidation** (with automatic MVC validation)
- **Serilog** (console sink)
- **Swashbuckle / Swagger** (with a JWT-aware "Authorize" button)
- **Docker** / Docker Compose for local orchestration

## Architecture & technical decisions

The solution is split into three projects to keep concerns isolated and
the codebase scalable as it grows:

- **`ConferenceRoomBooking.Api`** — HTTP layer only: controllers, DI/
  pipeline configuration, Swagger setup, startup seeders.
- **`ConferenceRoomBooking.Application`** — business logic: orchestrators
  (one per feature area), DTOs, FluentValidation validators, mappers,
  pricing/availability calculators, JWT issuing, and typed exceptions.
- **`ConferenceRoomBooking.DataLayer`** — EF Core `DbContext`, entity
  configurations, migrations, and reusable IQueryable extensions.

Notable decisions in service of clean, scalable, secure code:

- **Controllers stay thin** — they only translate HTTP ↔ orchestrator
  calls; all business rules live in `Application` orchestrators, each
  behind an interface for testability.
- **Centralized error handling** — a single `GlobalExceptionHandler` maps
  typed exceptions (`NotFoundException`, `ConflictException`,
  `RoomUnavailableException`, `UnauthorizedException`) to the correct
  HTTP status + RFC 7807 `ProblemDetails` response, so controllers never
  handle errors individually.
- **Validation is declarative** — every request DTO has a matching
  FluentValidation validator, run automatically before the action executes.
- **AuthN/AuthZ** — JWT bearer tokens issued on register/login; endpoints
  are locked down by role-based policies (`RequireUser`, `RequireAdmin`)
  rather than ad-hoc role checks.
- **Soft delete for rooms** — deleting a room with existing bookings
  doesn't destroy historical booking/revenue data; it's flagged
  `IsDeleted` and excluded from queries via a reusable query extension.
- **Idempotent startup seeding** — reference data (rooms, services) and
  role seeding only insert what's missing, so it's safe to restart the
  container repeatedly.

## Running with Docker

### Prerequisites
- [Docker](https://www.docker.com/) with Compose support (Docker Desktop,
  or Docker Engine + the Compose plugin).
- The [.NET SDK](https://dotnet.microsoft.com/download) *on the host*, only
  needed to run EF Core migrations (see step 2) — the API itself runs
  entirely inside the container.

### 1. Start the containers

From the repository root:

```bash
docker compose up --build -d
```

This builds the API image (`ConferenceRoomBooking.Api/Dockerfile`, a
multi-stage SDK-build → ASP.NET-runtime image) and starts two containers:

| Service      | Container name                     | Exposed on             |
|--------------|-------------------------------------|-------------------------|
| API          | `conferenceroombooking.api`         | `http://localhost:5000` |
| SQL Server   | `conferenceroombooking.sqlserver`   | `localhost:1433`        |

`compose.yaml` runs the API with `ASPNETCORE_ENVIRONMENT=Development`, so
Swagger UI is enabled even in the containerized build.

### 2. Apply database migrations (required)

The container does **not** run migrations automatically. Before (or
right after) the first `docker compose up`, apply them from the host once
the SQL Server container is healthy:

```bash
dotnet tool install --global dotnet-ef   # first time only
dotnet ef database update `
  --project ConferenceRoomBooking.DataLayer `
  --startup-project ConferenceRoomBooking.Api
```

(On bash/macOS/Linux, replace the trailing `` ` `` line-continuations with
`\`.) This uses the connection string in `appsettings.Development.json`,
which already points at `localhost,1433` — matching the port SQL Server
publishes to the host.

Once the schema exists, restart the API container so startup seeding can
run against it:

```bash
docker compose restart conferenceroombooking.api
```

On startup the API seeds roles (`User`, `Admin`), the 3 rooms, 3 services,
and a handful of sample bookings — see [Seed data](#seed-data). Seeding is
safe to run repeatedly; it only inserts what's missing.

### 3. Explore the API

- Swagger UI: **http://localhost:5000/swagger**
- Base URL: **http://localhost:5000/api**

### Stopping / resetting

```bash
docker compose down        # stop containers, keep the DB volume
docker compose down -v     # stop containers and wipe the DB volume
```

> ⚠️ **Dev-only configuration.** `compose.yaml` hardcodes the SQL Server
> `SA` password and forces `ASPNETCORE_ENVIRONMENT=Development` (which
> also disables HTTPS redirection and exposes Swagger). This is fine for
> local development/demo purposes but must not be reused as-is for a
> shared or production deployment — replace the password and JWT signing
> key (currently in `appsettings.Development.json`, clearly marked as
> dev-only) with real secrets.

## Authentication quick-start

Most endpoints require a JWT bearer token. Two endpoints are open:

1. **Register**: `POST /api/auth/register` with `{ "email": "...", "password": "..." }`
   → creates the account (assigned the `User` role) and returns an
   `accessToken`.
2. **Login**: `POST /api/auth/login` with the same shape → returns a fresh
   `accessToken`.

To call protected endpoints in Swagger: click **Authorize**, paste the
token **without** the `Bearer ` prefix (Swagger adds it automatically),
and confirm. Outside Swagger, send it as `Authorization: Bearer <token>`.

> There's no pre-seeded admin account — registration always grants the
> `User` role. Every business endpoint (rooms, bookings, services,
> analytics) only requires the `User`-or-`Admin` policy, so a normal
> registered account can exercise the whole API. An `Admin`-only policy
> exists in code (`AuthorizationPolicies.RequireAdmin`) for future
> endpoints that should be restricted further.

## API endpoint breakdown

All routes are prefixed with `/api`. 🔑 = requires a bearer token (`User`
or `Admin` role). ⭐ = one of the assignment's 5 core methods.

### Auth (`/api/auth`) — public

| Method | Route              | Description                          |
|--------|---------------------|---------------------------------------|
| POST   | `/auth/register`    | Create an account, returns a JWT      |
| POST   | `/auth/login`        | Authenticate, returns a JWT           |

### Rooms (`/api/rooms`) 🔑

| Method | Route              | Description                                              |
|--------|---------------------|------------------------------------------------------------|
| POST   | `/rooms`             | ⭐ Create a room (name, capacity, base hourly rate)         |
| GET    | `/rooms/{roomId}`    | Get a room by ID                                          |
| PUT    | `/rooms/{roomId}`    | ⭐ Update a room's rate, capacity, or offered services       |
| DELETE | `/rooms/{roomId}`    | ⭐ Soft-delete a room                                       |
| GET    | `/rooms/available`   | ⭐ Search rooms free for a date/time window with enough capacity |

### Bookings (`/api/bookings`) 🔑

| Method | Route                  | Description                                                          |
|--------|-------------------------|-------------------------------------------------------------------------|
| POST   | `/bookings`             | ⭐ Book a room (room, start time, duration, services) → returns the booking with a full price breakdown |
| GET    | `/bookings/{bookingId}` | Get a booking by ID                                                    |

### Service options (`/api/serviceoptions`) 🔑

| Method | Route                        | Description                              |
|--------|-------------------------------|--------------------------------------------|
| POST   | `/serviceoptions`             | Create a service option (e.g. Projector)  |
| GET    | `/serviceoptions/{id}`        | Get a service option by ID                |
| PUT    | `/serviceoptions/{id}`        | Update a service option                   |
| DELETE | `/serviceoptions/{id}`        | Delete a service option (blocked if in use by a room) |
| GET    | `/serviceoptions`              | Search / list service options             |

### Analytics (`/api/analytics`) 🔑

See [Reports & analytics](#reports--analytics) below.

## Reports & analytics

Two read-only reporting endpoints, aimed at giving the business insight
into room and service performance:

| Method | Route                              | What it returns                                                                 |
|--------|--------------------------------------|-----------------------------------------------------------------------------------|
| GET    | `/api/analytics/room-performance`    | Per room: total bookings, total revenue, average booking duration, revenue rank |
| GET    | `/api/analytics/service-performance` | Per service: times selected, distinct rooms it was used in, total revenue, revenue rank |

## Seed data

On every startup the API seeds (idempotently — existing rows are left
untouched):

- **Roles**: `User`, `Admin`
- **Rooms**: Room A (50 people, 2000₴/hour), Room B (100 people,
  3500₴/hour), Room C (30 people, 1500₴/hour)
- **Services**: Projector (500₴), Wi-Fi (300₴), Sound (700₴)
- **~9 sample bookings** spread across the rooms and services over the
  past month, with prices already computed by the same pricing engine
  used at runtime — useful for trying the analytics endpoints immediately.

## Local development without Docker

1. Start a local or containerized SQL Server instance on `localhost,1433`
   (or update `ConnectionStrings:DefaultConnection` in
   `appsettings.Development.json`).
2. Apply migrations:
   ```bash
   dotnet ef database update --project ConferenceRoomBooking.DataLayer --startup-project ConferenceRoomBooking.Api
   ```
3. Run the API:
   ```bash
   dotnet run --project ConferenceRoomBooking.Api
   ```
4. Swagger UI is served at the URL printed on startup (see
   `Properties/launchSettings.json`, e.g. `http://localhost:5173/swagger`).
