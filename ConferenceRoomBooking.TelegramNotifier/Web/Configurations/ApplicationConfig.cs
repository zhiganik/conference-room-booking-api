namespace ConferenceRoomBooking.TelegramNotifier.Web.Configurations;

public static class ApplicationConfig
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Telegram Notifier API v1");
            });
        }

        app.MapControllers();

        return app;
    }
}
