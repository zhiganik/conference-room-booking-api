using ConferenceRoomBooking.Bll.Common.Notifications;
using ConferenceRoomBooking.Bll.Common.Reports;
using ConferenceRoomBooking.Utils.Storage;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBooking.Bll.Reports;

public class HourlyBookingReportPublisher(
    IBookingReportManager reportManager,
    IBlobStorageClient blobStorageClient,
    INotifier notifier,
    ILogger<HourlyBookingReportPublisher> logger) : IHourlyBookingReportPublisher
{
    private const string ContainerName = "hourly-booking-reports";

    public async Task PublishAsync(DateTime periodStartUtc, DateTime periodEndUtc, CancellationToken cancellationToken)
    {
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
