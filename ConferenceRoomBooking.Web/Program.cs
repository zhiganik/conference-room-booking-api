using ConferenceRoomBooking.Dal.SqlRepositories.Migrations;
using ConferenceRoomBooking.Web.Configurations;
using ConferenceRoomBooking.Web.Startup;
using DotNetEnv;
using Microsoft.ApplicationInsights;

Env.Load(options: LoadOptions.TraversePath().NoClobber());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
var app = builder.Build();

app.UseApplicationPipeline();

try
{
    DatabaseMigrator.Migrate(app.Configuration);
    await DataSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    app.Services.GetRequiredService<ILogger<Program>>()
        .LogCritical(ex, "Application startup failed; the application cannot serve requests.");

    // The process is about to crash - give the telemetry channel a chance to send this
    // Critical log before that happens, otherwise it's lost.
    app.Services.GetService<TelemetryClient>()?.Flush();
    await Task.Delay(TimeSpan.FromSeconds(2));

    throw;
}

app.Run();
