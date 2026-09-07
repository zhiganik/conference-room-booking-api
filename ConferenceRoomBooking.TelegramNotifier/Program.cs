using ConferenceRoomBooking.TelegramNotifier.Dal.Migrations;
using ConferenceRoomBooking.TelegramNotifier.Web.Configurations;
using DotNetEnv;
using Serilog;

Env.Load(options: LoadOptions.NoClobber());

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddDependencies(builder.Configuration);
    var app = builder.Build();

    app.UseApplicationPipeline();

    AlertSubscribersMigrator.Migrate(app.Configuration);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ConferenceRoomBooking.TelegramNotifier terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
