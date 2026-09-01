using ConferenceRoomBooking.Dal.SqlRepositories.Migrations;
using ConferenceRoomBooking.Web.Configurations;
using ConferenceRoomBooking.Web.Startup;
using Serilog;

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

    DatabaseMigrator.Migrate(app.Configuration);
    await DataSeeder.SeedAsync(app.Services);
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ConferenceRoomBooking.Web terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}