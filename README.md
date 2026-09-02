# Conference Room Booking API

A simple, secure API for managing conference rooms, bookings, and rental
pricing — built with ASP.NET Core (.NET 10), raw ADO.NET, and Azure SQL.

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
- **Raw ADO.NET** (`Microsoft.Data.SqlClient`) — no EF Core, no ORM;
  every query is `CommandType.StoredProcedure` only (no inline SQL, no
  dynamic SQL)
- **Azure SQL Database**, authenticated via **Microsoft Entra ID**
  (`Azure.Identity`'s `DefaultAzureCredential`) — no SQL logins or
  passwords anywhere in configuration
- **DbUp** — forward-only, journal-tracked schema migrations plus
  stored-procedure scripts, both run automatically on app startup (no
  separate publish/deploy step)
- **Custom `Users` table** + **JWT bearer authentication** (no ASP.NET
  Identity), PBKDF2 password hashing, role-based authorization policies
- **AutoMapper** (Entity ↔ Model in the data layer, Model ↔ Dto in the
  web layer)
- **FluentValidation** (with automatic MVC validation)
- **Serilog** (console sink)
- **Swashbuckle / Swagger** (with a JWT-aware "Authorize" button)
- **Docker** / Docker Compose for containerized runs

## Architecture & technical decisions

The solution is split into four projects along Service/BLL/Common/DAL
lines, to keep concerns isolated and the codebase scalable as it grows:

- **`ConferenceRoomBooking.Web`** — HTTP layer only: controllers, DI/
  pipeline configuration, Swagger setup, and the startup hooks that run
  DB migrations and seed data.
- **`ConferenceRoomBooking.Bll`** — business logic: managers (one per
  feature area, e.g. `RoomManager`, `BookingManager`), JWT issuing,
  password hashing, the rental-price calculator, and the per-room
  booking lock.
- **`ConferenceRoomBooking.Bll.Common`** — shared contracts only: domain
  models, manager/repository interfaces, typed exceptions, and
  cross-cutting security/settings/abstractions. Depends on nothing else
  in the solution, so it's safe for both `Bll` and
  `Dal.SqlRepositories` to depend on it without a circular reference.
- **`ConferenceRoomBooking.Dal.SqlRepositories`** — persistence:
  ADO.NET repositories (stored procedures only), the DbUp migration
  scripts (`Migrations/Scripts/01_Migrations` for schema, `02_Procedures`
  for stored procedures), and the Entity ↔ Model AutoMapper profile.

Dependency direction is one-way: `Web` → `Bll`, `Bll.Common`,
`Dal.SqlRepositories` (the last one only for DI wiring); `Bll` →
`Bll.Common`; `Dal.SqlRepositories` → `Bll.Common`; `Bll.Common` depends
on nothing.

Notable decisions in service of clean, scalable, secure code:

- **Controllers stay thin** — they only translate HTTP ↔ manager calls;
  all business rules live in `Bll` managers, each behind an interface
  for testability.
- **No ORM, stored procedures only** — every repository method calls a
  named stored procedure via `CommandType.StoredProcedure`; there's no
  inline or dynamically-built SQL anywhere in the app, which keeps every
  query DBA-reviewable and closes off SQL injection as an attack surface
  entirely.
- **Migrations run themselves** — DbUp applies pending scripts on every
  startup: schema/seed-data scripts run exactly once (journal-tracked),
  stored-procedure scripts (`CREATE OR ALTER`) re-run every startup, so
  editing a procedure in place is safe and normal.
- **Azure SQL + Entra ID, no passwords** — the connection string carries
  no credentials; `DefaultAzureCredential` resolves whichever identity
  is available (see [Prerequisites](#prerequisites) below for what that
  means for local runs).
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
  `IsDeleted` and every procedure that reads `Rooms` filters it out
  explicitly (there's no ORM query filter to do it implicitly).
- **Startup seeding** — reference data (rooms, services, two demo users)
  and a handful of sample bookings are inserted on every startup. Users
  and service options are idempotent (skip-if-already-present); rooms
  and sample bookings currently are not — see
  [Seed data](#seed-data) for the practical implication.

## Prerequisites

- The [.NET 10 SDK](https://dotnet.microsoft.com/download).
- Network access to the target Azure SQL Database
  (`rg-academy-6.database.windows.net`, configured in
  `appsettings.Development.json`), and an Azure identity with rights on
  it. `DefaultAzureCredential` tries several credential sources in
  order — the one that matters for local development is **whichever
  account you're signed into in Visual Studio** (or `az login` via the
  Azure CLI, if you use that instead). This is the setup this project
  has actually been run and verified against — see
  [Running locally](#running-locally).
- [Docker](https://www.docker.com/) with Compose support, only if you
  want the containerized run — see the caveat under
  [Running with Docker](#running-with-docker) before relying on it.

## Running locally

This is the path that's actually been used to run and verify this
project end-to-end.

1. Make sure Visual Studio (or the Azure CLI, via `az login`) is signed
   into an Azure account that has access to `rg-academy-6`.
2. Run the API:
   ```bash
   dotnet run --project ConferenceRoomBooking.Web
   ```
   (or press F5 in Visual Studio).
3. On startup the app connects to Azure SQL using that identity, runs
   any pending DbUp migrations/procedure scripts automatically, seeds
   reference + demo data (see [Seed data](#seed-data)), and starts
   listening — no separate migration or publish step.
4. Swagger UI is served at the URL printed on startup (see
   `ConferenceRoomBooking.Web/Properties/launchSettings.json`, e.g.
   `http://localhost:5173/swagger`).

## Running with Docker

```bash
docker compose up --build -d
```

This builds a single image (`ConferenceRoomBooking.Web/Dockerfile`,
multi-stage SDK-build → ASP.NET-runtime) and starts one container:

| Service      | Container name                 | Exposed on              |
|--------------|----------------------------------|---------------------------|
| API          | `conferenceroombooking.web`      | `http://localhost:5000`  |

`docker-compose.yml` runs it with `ASPNETCORE_ENVIRONMENT=Development`,
so Swagger UI is enabled. There's no separate database container —
migrations, seeding, and the app itself all run inside this one
container, the same way they do locally.

> ⚠️ **Azure authentication inside the container is unverified.**
> `DefaultAzureCredential` needs a reachable identity source, and this
> project's compose files don't mount an Azure CLI/Visual Studio
> credential cache or set service-principal environment variables
> (`AZURE_CLIENT_ID` / `AZURE_TENANT_ID` / `AZURE_CLIENT_SECRET`) for
> the container to authenticate with. This project has only actually
> been run via [`dotnet run`/Visual Studio](#running-locally) so far. If
> you need the container path to work, add one of those credential
> sources to `docker-compose.override.yml` first.

Swagger UI: **http://localhost:5000/swagger** · Base URL:
**http://localhost:5000/api**

### Stopping

```bash
docker compose down
```

> ⚠️ **Dev-only configuration.** `docker-compose.yml` forces
> `ASPNETCORE_ENVIRONMENT=Development` (which also disables HTTPS
> redirection and exposes Swagger), and the JWT signing key in
> `appsettings.Development.json` is a placeholder committed for local
> convenience (explicitly marked as dev-only in that file, to avoid
> needing a `.env`/user-secrets setup just to try the project). Neither
> is fit for a shared or production deployment as-is.

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

> Registration always grants the `User` role — there's no self-service
> way to become `Admin`. A pre-seeded admin account is available instead
> (see [Seed data](#seed-data)) for exercising `Admin`-only endpoints
> (currently just Analytics and write access to Rooms/Service options).

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
| GET    | `/bookings/my`          | Get the current user's own bookings                                    |

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
into room and service performance. Backed by SQL views
(`RoomPerformanceView` / `ServicePerformanceView`) rather than
per-request aggregation, so the ranking logic (`RANK() OVER`) lives in
one place in the schema.

| Method | Route                              | What it returns                                                                 |
|--------|--------------------------------------|-----------------------------------------------------------------------------------|
| GET    | `/api/analytics/room-performance`    | Per room: total bookings, total revenue, average booking duration, revenue rank |
| GET    | `/api/analytics/service-performance` | Per service: times selected, distinct rooms it was used in, total revenue, revenue rank |

## Seed data

On every startup the API seeds:

- **Users**: a `seed.bookings@conference-room-booking.local` /
  `Seed@Bookings123!` account (`User` role, owns the sample bookings
  below) and an `admin@gmail.com` / `Admin1234!` account (`Admin` role,
  the one pre-seeded way to reach `Admin`-only endpoints). Both are
  idempotent — skipped if the email already exists.
- **Rooms**: Room A (50 people, 2000₴/hour), Room B (100 people,
  3500₴/hour), Room C (30 people, 1500₴/hour).
- **Services**: Projector (500₴), Wi-Fi (300₴), Sound (700₴). Idempotent
  — skipped if a service with that name already exists.
- **~9 sample bookings** spread across the rooms and services over the
  past month, with prices already computed by the same pricing engine
  used at runtime — useful for trying the analytics endpoints immediately.

> ⚠️ **Rooms and sample bookings are not yet idempotent** — unlike users
> and services, nothing currently guards against re-inserting them, so
> restarting the app against a database that already has this seed data
> will create duplicates. Safe to rely on against a freshly-migrated,
> empty database; not safe to restart repeatedly against one that's
> already been seeded.
