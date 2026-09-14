using Azure.Monitor.OpenTelemetry.Exporter;
using ConferenceRoomBooking.Bll;
using ConferenceRoomBooking.Dal.BlobsStorage;
using ConferenceRoomBooking.Dal.SqlRepositories;
using ConferenceRoomBooking.Dal.SqlRepositories.Migrations;
using ConferenceRoomBooking.Functions.Configurations;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddDalSqlRepositories()
    .AddDalBlobsStorage(builder.Configuration)
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

DatabaseMigrator.Migrate(app.Services.GetRequiredService<IConfiguration>());

app.Run();
