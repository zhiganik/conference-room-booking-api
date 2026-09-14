using ConferenceRoomBooking.Bll.Common.Notifications;
using ConferenceRoomBooking.Bll.Common.Reports;
using ConferenceRoomBooking.Utils.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Functions.Functions;

public class HourlyBookingReportFunction(
    IBookingReportManager reportManager,
    IBlobStorageClient blobStorageClient,
    INotifier notifier,
    ILogger<HourlyBookingReportFunction> logger)
{
    private const string ContainerName = "hourly-booking-reports";

    [Function("HourlyBookingReportFunction")]
    public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        var periodEndUtc = DateTime.UtcNow;
        var periodStartUtc = timer.ScheduleStatus?.Last ?? periodEndUtc.AddHours(-1);

        try
        {
            var report = await reportManager.GenerateHourlyReportAsync(periodStartUtc, periodEndUtc, cancellationToken);

            var blobName = $"{periodEndUtc:yyyy-MM-dd_HH-mm-ss}.json";
            await blobStorageClient.EnsureContainerExistsAsync(ContainerName, cancellationToken);
            await blobStorageClient.UploadJsonAsync(ContainerName, blobName, report, cancellationToken: cancellationToken);

            var reportUrl = await blobStorageClient.GetTemporaryReadUrlAsync(
                ContainerName, blobName, TimeSpan.FromHours(3), cancellationToken);

            var summary = $"Hourly booking report {periodStartUtc:HH:mm}-{periodEndUtc:HH:mm} UTC: " +
                          $"{report.TotalBookings} booking(s), {report.TotalRevenue:C} total.\n{reportUrl}";
            await notifier.PushAsync(summary, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hourly booking report failed");
        }
    }
}
