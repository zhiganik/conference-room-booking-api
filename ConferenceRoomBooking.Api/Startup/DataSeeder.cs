using ConferenceRoomBooking.Application.BusinessLogic.Pricing;
using ConferenceRoomBooking.Application.Security;
using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Startup;

public static class DataSeeder
{
    private const string SeedUserEmail = "seed.bookings@conference-room-booking.local";
    private const string SeedUserPassword = "Seed@Bookings123!";

    private const string AdminEmail = "admin@gmail.com";
    private const string AdminPassword = "Admin1234!";

    private static readonly (string Name, int Capacity, decimal BaseHourlyRate)[] Rooms =
    [
        ("Room A", 50, 2000m),
        ("Room B", 100, 3500m),
        ("Room C", 30, 1500m)
    ];

    private static readonly (string Name, decimal Price)[] ServiceOptions =
    [
        ("Projector", 500m),
        ("Wi-Fi", 300m),
        ("Sound", 700m)
    ];

    private static readonly (string RoomName, int DaysAgo, TimeSpan Start, int DurationMinutes, string[] ServiceNames)[]
        BookingDefinitions =
        [
            ("Room A", 30, new TimeSpan(7, 0, 0), 120, ["Wi-Fi"]),
            ("Room A", 25, new TimeSpan(10, 0, 0), 90, ["Projector", "Wi-Fi"]),
            ("Room A", 18, new TimeSpan(12, 30, 0), 60, []),
            ("Room A", 10, new TimeSpan(19, 0, 0), 120, ["Projector", "Wi-Fi", "Sound"]),

            ("Room B", 28, new TimeSpan(11, 0, 0), 180, ["Projector"]),
            ("Room B", 20, new TimeSpan(13, 0, 0), 60, ["Sound"]),
            ("Room B", 12, new TimeSpan(18, 30, 0), 90, ["Projector", "Sound"]),

            ("Room C", 22, new TimeSpan(6, 30, 0), 90, []),
            ("Room C", 15, new TimeSpan(11, 0, 0), 120, ["Wi-Fi", "Sound"])
        ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var priceCalculator = scope.ServiceProvider.GetRequiredService<IRentalPriceCalculator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        await SeedRoomsAsync(dbContext, logger);
        await SeedServiceOptionsAsync(dbContext, logger);

        var seedUserId = await EnsureUserAsync(userManager, SeedUserEmail, SeedUserPassword, Roles.User, logger);
        await EnsureUserAsync(userManager, AdminEmail, AdminPassword, Roles.Admin, logger);

        await SeedBookingsAsync(dbContext, priceCalculator, seedUserId, logger);
    }

    private static async Task<string> EnsureUserAsync(UserManager<AppUser> userManager, string email, string password, string role, ILogger logger)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return user.Id;
        }

        user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
        }

        await userManager.AddToRoleAsync(user, role);

        logger.LogInformation("Seeded {Role} user '{Email}'.", role, email);
        return user.Id;
    }

    private static async Task SeedRoomsAsync(AppDbContext dbContext, ILogger logger)
    {
        var existingNames = await dbContext.Rooms.Select(r => r.Name).ToListAsync();

        var toInsert = Rooms
            .Where(r => !existingNames.Contains(r.Name))
            .Select(r => new Room
            {
                Name = r.Name,
                Capacity = r.Capacity,
                BaseHourRate = r.BaseHourlyRate,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        if (toInsert.Count == 0)
        {
            return;
        }

        dbContext.Rooms.AddRange(toInsert);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} room(s): {Names}",
            toInsert.Count, string.Join(", ", toInsert.Select(r => r.Name)));
    }

    private static async Task SeedServiceOptionsAsync(AppDbContext dbContext, ILogger logger)
    {
        var existingNames = await dbContext.ServiceOptions.Select(s => s.Name).ToListAsync();

        var toInsert = ServiceOptions
            .Where(s => !existingNames.Contains(s.Name))
            .Select(s => new ServiceOption { Name = s.Name, Price = s.Price })
            .ToList();

        if (toInsert.Count == 0)
        {
            return;
        }

        dbContext.ServiceOptions.AddRange(toInsert);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} service(s): {Names}",
            toInsert.Count, string.Join(", ", toInsert.Select(s => s.Name)));
    }
    
    private static async Task SeedBookingsAsync(AppDbContext dbContext, IRentalPriceCalculator priceCalculator, string seedUserId, ILogger logger)
    {
        if (await dbContext.Bookings.AnyAsync())
        {
            return;
        }

        var rooms = await dbContext.Rooms.ToDictionaryAsync(r => r.Name);
        var serviceOptions = await dbContext.ServiceOptions.ToDictionaryAsync(s => s.Name);

        var bookings = new List<Booking>();

        foreach (var def in BookingDefinitions)
        {
            if (!rooms.TryGetValue(def.RoomName, out var room))
            {
                logger.LogWarning("Skipped seeding a booking: room '{RoomName}' not found.", def.RoomName);
                continue;
            }

            var startTime = DateTime.UtcNow.Date.AddDays(-def.DaysAgo).Add(def.Start);
            var endTime = startTime.AddMinutes(def.DurationMinutes);

            var selectedServices = def.ServiceNames
                .Where(serviceOptions.ContainsKey)
                .Select(name => serviceOptions[name])
                .ToList();

            var priceBreakdown = priceCalculator.Calculate(
                room.BaseHourRate, startTime, endTime, selectedServices.Select(s => s.Price));

            var booking = new Booking
            {
                RoomId = room.Id,
                RoomName = room.Name,
                UserId = seedUserId,
                StartTime = startTime,
                EndTime = endTime,
                BaseRoomCost = priceBreakdown.BaseRoomCost,
                ServicesCost = priceBreakdown.ServicesCost,
                TotalPrice = priceBreakdown.TotalPrice,
                CreatedAtUtc = DateTime.UtcNow
            };

            // Add via navigation, NOT by setting BookingId directly — booking.Id
            // is 0 (unassigned) at this point since it's an identity column.
            foreach (var service in selectedServices)
            {
                booking.BookingServiceOptions.Add(new BookingServiceOption
                {
                    ServiceOptionId = service.Id,
                    ServiceOptionName = service.Name,
                    PriceAtBooking = service.Price
                });
            }

            bookings.Add(booking);
        }

        if (bookings.Count == 0)
        {
            return;
        }

        // Adding only the Booking entities is enough — EF Core walks the
        // BookingServiceOptions navigation automatically and inserts those too.
        dbContext.Bookings.AddRange(bookings);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} booking(s) across {RoomCount} room(s).",
            bookings.Count, bookings.Select(b => b.RoomId).Distinct().Count());
    }
}