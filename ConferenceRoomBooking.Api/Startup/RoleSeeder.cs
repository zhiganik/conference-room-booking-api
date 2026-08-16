using ConferenceRoomBooking.Application.Security;
using Microsoft.AspNetCore.Identity;

namespace ConferenceRoomBooking.Api.Startup;

public static  class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        foreach (var roleName in Roles.Get())
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole { Name = roleName });
            if (result.Succeeded)
            {
                logger.LogInformation("Seeded role: {RoleName}", roleName);
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed role {RoleName}: {Errors}", roleName, errors);
            }
        }
    }
}