using ConferenceRoomBooking.Bll.Common.Notifications;
using ConferenceRoomBooking.Bll.Common.Reports;
using ConferenceRoomBooking.Utils.Storage;
using ConferenceRoomBooking.Web.Configurations;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Web.BackgroundServices;

public class HourlyBookingReportBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<HourlyBookingReportSettings> options,
    ILogger<HourlyBookingReportBackgroundService> logger) : BackgroundService
{
    private const string ContainerName = "hourly-booking-reports";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
        using var timer = new PeriodicTimer(interval);

        do
        {
            try
            {
                await GenerateAndPublishReportAsync(interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Hourly booking report failed");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task GenerateAndPublishReportAsync(TimeSpan interval, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var reportManager = services.GetRequiredService<IBookingReportManager>();
        var blobStorageClient = services.GetRequiredService<IBlobStorageClient>();
        var notifier = services.GetRequiredService<INotifier>();

        var periodEndUtc = DateTime.UtcNow;
        var periodStartUtc = periodEndUtc - interval;

        var report = await reportManager.GenerateHourlyReportAsync(periodStartUtc, periodEndUtc, cancellationToken);

        var blobName = $"{periodEndUtc:yyyy-MM-dd_HH-mm-ss}.json";
        await blobStorageClient.EnsureContainerExistsAsync(ContainerName, cancellationToken);
        await blobStorageClient.UploadJsonAsync(ContainerName, blobName, report, cancellationToken: cancellationToken);

        var summary = $"Hourly booking report {periodStartUtc:HH:mm}-{periodEndUtc:HH:mm} UTC: " +
                      $"{report.TotalBookings} booking(s), {report.TotalRevenue:C} total.";
        await notifier.PushAsync(summary, cancellationToken);

        logger.LogInformation("Published hourly booking report {BlobName}", blobName);
    }
}
