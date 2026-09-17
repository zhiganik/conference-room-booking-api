using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace ConferenceRoomBooking.Web.Configurations;

public static class ApplicationConfig
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseAzureAppConfiguration();
        app.UseExceptionHandler();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Room Booking API v1");
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();

        app.Use(async (context, next) =>
        {
            var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                Activity.Current?.SetTag("enduser.id", userId);
            }

            await next(context);
        });

        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}