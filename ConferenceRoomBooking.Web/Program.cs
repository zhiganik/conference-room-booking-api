using Azure.Identity;
using ConferenceRoomBooking.Dal.SqlRepositories.Migrations;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using ConferenceRoomBooking.Web.Configurations;
using ConferenceRoomBooking.Web.Startup;
using Microsoft.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

var appConfigEndpoint = builder.Configuration["AppConfig:Endpoint"]
    ?? throw new InvalidOperationException("AppConfig:Endpoint is not configured.");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var credential = new DefaultAzureCredential();

    options.Connect(new Uri(appConfigEndpoint), credential)
        .ConfigureKeyVault(kv =>
        {
            kv.SetCredential(credential);
            kv.SetSecretRefreshInterval(TimeSpan.FromMinutes(1));
        })
        .Select(KeyFilter.Any, LabelFilter.Null)
        .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
        .ConfigureRefresh(refresh => refresh
            .Register("Sentinel", refreshAll: true)
            .SetRefreshInterval(TimeSpan.FromMinutes(1)));
});

builder.Services.AddAzureAppConfiguration();
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

    app.Services.GetService<TelemetryClient>()?.Flush();
    await Task.Delay(TimeSpan.FromSeconds(2));
    throw;
}

app.Run();
