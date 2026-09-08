using ConferenceRoomBooking.Bll.Common.Reports;
using ConferenceRoomBooking.Web.Configurations;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Web.BackgroundServices;

public class HourlyBookingReportBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<HourlyBookingReportSettings> options,
    ILogger<HourlyBookingReportBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
        using var timer = new PeriodicTimer(interval);

        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<IHourlyBookingReportPublisher>();

                var periodEndUtc = DateTime.UtcNow;
                var periodStartUtc = periodEndUtc - interval;

                await publisher.PublishAsync(periodStartUtc, periodEndUtc, stoppingToken);
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
}
