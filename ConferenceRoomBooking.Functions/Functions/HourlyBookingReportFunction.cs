using System.Net;
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
    private static readonly TimeSpan ReportWindow = TimeSpan.FromDays(1);

    [Function("HourlyBookingReportFunction")]
    public async Task Run([TimerTrigger("0 0 9 * * *")] TimerInfo timer, CancellationToken cancellationToken)
    {
        var periodEndUtc = DateTime.UtcNow;
        var periodStartUtc = GetPeriodStartUtc(timer, periodEndUtc);

        logger.LogInformation(
            "Hourly booking report period: {PeriodStartUtc:O} - {PeriodEndUtc:O} (ScheduleStatus.Last: {ScheduleLast:O})",
            periodStartUtc,
            periodEndUtc,
            timer.ScheduleStatus?.Last);

        try
        {
            await PublishReportAsync(periodStartUtc, periodEndUtc, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hourly booking report failed");
            throw;
        }
    }

    private static DateTime GetPeriodStartUtc(TimerInfo timer, DateTime periodEndUtc)
    {
        var lastRun = timer.ScheduleStatus?.Last;
        var isValidLastRun = lastRun.HasValue
            && lastRun.Value < periodEndUtc
            && periodEndUtc - lastRun.Value <= ReportWindow;

        return isValidLastRun
            ? lastRun!.Value
            : periodEndUtc - ReportWindow;
    }

    private async Task PublishReportAsync(
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        CancellationToken cancellationToken)
    {
        var report = await reportManager
            .GenerateHourlyReportAsync(periodStartUtc, periodEndUtc, cancellationToken)
            .ConfigureAwait(false);

        var blobName = $"{periodEndUtc:yyyy-MM-dd_HH-mm-ss}.json";
        await blobStorageClient
            .EnsureContainerExistsAsync(ContainerName, cancellationToken)
            .ConfigureAwait(false);
        await blobStorageClient
            .UploadJsonAsync(ContainerName, blobName, report, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var reportUrl = await blobStorageClient
            .GetTemporaryReadUrlAsync(ContainerName, blobName, TimeSpan.FromHours(3), cancellationToken)
            .ConfigureAwait(false);

        var summary = $"Hourly booking report {periodStartUtc:dd.MM.yyyy HH:mm}-{periodEndUtc:dd.MM.yyyy HH:mm} UTC: "
            + $"{report.TotalBookings} booking(s), {report.TotalRevenue:C} total.\n"
            + $"<a href=\"{WebUtility.HtmlEncode(reportUrl.ToString())}\">Report</a>";

        await notifier.PushAsync(summary, cancellationToken).ConfigureAwait(false);
    }
}
