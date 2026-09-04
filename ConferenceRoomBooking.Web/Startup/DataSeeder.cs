using ConferenceRoomBooking.Bll.Auth;
using ConferenceRoomBooking.Bll.Bookings;
using ConferenceRoomBooking.Bll.Common.Auth;
using ConferenceRoomBooking.Bll.Common.Auth.Models;
using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Security;

namespace ConferenceRoomBooking.Web.Startup;

public static class DataSeeder
{
    private const string SeedUserEmail = "seed.bookings@conference-room-booking.local";
    private const string SeedUserPassword = "Seed@Bookings123!";

    private const string AdminEmail = "admin@gmail.com";
    private const string AdminPassword = "Admin1234!";

    private static readonly (string Name, int Capacity, decimal BaseHourlyRate)[] RoomDefinitions =
    [
        ("Room A", 50, 2000m),
        ("Room B", 100, 3500m),
        ("Room C", 30, 1500m)
    ];

    private static readonly (string Name, decimal Price)[] ServiceOptionDefinitions =
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
        var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
        var serviceOptionRepository = scope.ServiceProvider.GetRequiredService<IServiceOptionRepository>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var priceCalculator = scope.ServiceProvider.GetRequiredService<IRentalPriceCalculator>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        var rooms = await SeedRoomsAsync(roomRepository, logger);
        var serviceOptions = await SeedServiceOptionsAsync(serviceOptionRepository, logger);

        var seedUser = await EnsureUserAsync(userRepository, passwordHasher, SeedUserEmail, SeedUserPassword, Roles.User, logger);
        await EnsureUserAsync(userRepository, passwordHasher, AdminEmail, AdminPassword, Roles.Admin, logger);

        await SeedBookingsAsync(bookingRepository, priceCalculator, rooms, serviceOptions, seedUser.Id, logger);
    }

    private static async Task<User> EnsureUserAsync(IUserRepository userRepository, IPasswordHasher passwordHasher,
        string email, string password, string role, ILogger logger)
    {
        var existing = await userRepository.GetByEmailAsync(email, CancellationToken.None);
        if (existing is not null)
        {
            return existing;
        }

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHasher.HashPassword(password),
            Role = role,
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await userRepository.CreateAsync(user, CancellationToken.None);
        logger.LogInformation("Seeded {Role} user '{Email}'.", role, email);
        return created;
    }

    private static async Task<Dictionary<string, Room>> SeedRoomsAsync(IRoomRepository roomRepository, ILogger logger)
    {
        var rooms = new Dictionary<string, Room>();
        var createdCount = 0;

        foreach (var definition in RoomDefinitions)
        {
            var existing = await roomRepository.GetByNameAsync(definition.Name, CancellationToken.None);
            if (existing is not null)
            {
                rooms[definition.Name] = existing;
                continue;
            }

            var room = await roomRepository.CreateAsync(new Room
            {
                Name = definition.Name,
                Capacity = definition.Capacity,
                BaseHourRate = definition.BaseHourlyRate
            }, CancellationToken.None);

            rooms[definition.Name] = room;
            createdCount++;
        }

        logger.LogInformation("Seeded {Count} new room(s) ({Total} total): {Names}",
            createdCount, rooms.Count, string.Join(", ", rooms.Keys));
        return rooms;
    }

    private static async Task<Dictionary<string, ServiceOption>> SeedServiceOptionsAsync(
        IServiceOptionRepository serviceOptionRepository, ILogger logger)
    {
        var serviceOptions = new Dictionary<string, ServiceOption>();
        var createdCount = 0;

        foreach (var definition in ServiceOptionDefinitions)
        {
            var existing = await serviceOptionRepository.GetByNameAsync(definition.Name, CancellationToken.None);
            if (existing is not null)
            {
                serviceOptions[definition.Name] = existing;
                continue;
            }

            var serviceOption = await serviceOptionRepository.CreateAsync(new ServiceOption
            {
                Name = definition.Name,
                Price = definition.Price
            }, CancellationToken.None);

            serviceOptions[definition.Name] = serviceOption;
            createdCount++;
        }

        logger.LogInformation("Seeded {Count} new service(s) ({Total} total): {Names}",
            createdCount, serviceOptions.Count, string.Join(", ", serviceOptions.Keys));
        return serviceOptions;
    }

    private static async Task SeedBookingsAsync(IBookingRepository bookingRepository, IRentalPriceCalculator priceCalculator,
        Dictionary<string, Room> rooms, Dictionary<string, ServiceOption> serviceOptions, Guid seedUserId, ILogger logger)
    {
        var count = 0;

        foreach (var definition in BookingDefinitions)
        {
            if (!rooms.TryGetValue(definition.RoomName, out var room))
            {
                logger.LogWarning("Skipped seeding a booking: room '{RoomName}' not found.", definition.RoomName);
                continue;
            }

            var startTime = DateTime.UtcNow.Date.AddDays(-definition.DaysAgo).Add(definition.Start);
            var endTime = startTime.AddMinutes(definition.DurationMinutes);

            if (await bookingRepository.ExistsOverlappingAsync(room.Id, startTime, endTime, CancellationToken.None))
            {
                continue;
            }

            var selectedServices = definition.ServiceNames
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
                CreatedAtUtc = DateTime.UtcNow,
                Services = selectedServices
                    .Select(s => new BookedServiceOption { ServiceOptionId = s.Id, Name = s.Name, PriceAtBooking = s.Price })
                    .ToList()
            };

            await bookingRepository.CreateAsync(booking, CancellationToken.None);
            count++;
        }

        logger.LogInformation("Seeded {Count} new booking(s).", count);
    }
}
