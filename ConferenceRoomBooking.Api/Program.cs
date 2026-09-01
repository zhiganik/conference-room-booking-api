using ConferenceRoomBooking.Api.Configurations;
using ConferenceRoomBooking.Api.Startup;
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
    
    await RoleSeeder.SeedRolesAsync(app.Services);
    await DataSeeder.SeedAsync(app.Services);
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ConferenceRoomBooking.Api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}