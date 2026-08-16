using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Startup;

public static class DataSeeder
{
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

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        await SeedRoomsAsync(dbContext, logger);
        await SeedServiceOptionsAsync(dbContext, logger);
    }

    private static async Task SeedRoomsAsync(AppDbContext dbContext, ILogger logger)
    {
        var existingNames = await dbContext.Rooms
            .Select(r => r.Name)
            .ToListAsync();

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
        var existingNames = await dbContext.ServiceOptions
            .Select(s => s.Name)
            .ToListAsync();

        var toInsert = ServiceOptions
            .Where(s => !existingNames.Contains(s.Name))
            .Select(s => new ServiceOption
            {
                Name = s.Name,
                Price = s.Price
            })
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
}