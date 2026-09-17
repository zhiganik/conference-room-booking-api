using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using ConferenceRoomBooking.Bll;
using ConferenceRoomBooking.Dal.BlobsStorage;
using ConferenceRoomBooking.Dal.SqlRepositories;
using ConferenceRoomBooking.Functions.Configurations;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var appConfigEndpoint = builder.Configuration["AppConfig:Endpoint"]
    ?? throw new InvalidOperationException("AppConfig:Endpoint is not configured.");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ExcludeManagedIdentityCredential = builder.Environment.IsDevelopment()
    });

    options.Connect(new Uri(appConfigEndpoint), credential)
        .ConfigureKeyVault(kv => kv.SetCredential(credential))
        .Select(KeyFilter.Any, LabelFilter.Null)
        .Select(KeyFilter.Any, builder.Environment.EnvironmentName);
});

builder.Services
    .AddAutoMapper(cfg => { }, typeof(ConferenceRoomBooking.Dal.SqlRepositories.Mapping.AutomapperConfig).Assembly)
    .AddDalSqlRepositories()
    .AddDalBlobsStorage(builder.Configuration)
    .AddFunctionsUserContext()
    .AddBusinessLogic()
    .AddNotificationsBusinessLogic()
    .AddTelegramChannel(builder.Configuration);

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

var app = builder.Build();

app.Run();
