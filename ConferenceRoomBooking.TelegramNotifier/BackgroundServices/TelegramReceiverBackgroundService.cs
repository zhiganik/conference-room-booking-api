using ConferenceRoomBooking.TelegramNotifier.Telegram;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace ConferenceRoomBooking.TelegramNotifier.BackgroundServices;

public class TelegramReceiverBackgroundService(
    ITelegramBotClient botClient,
    TelegramUpdateHandler updateHandler,
    ILogger<TelegramReceiverBackgroundService> logger) : BackgroundService
{
    private static readonly ReceiverOptions ReceiverOptions = new()
    {
        AllowedUpdates = [UpdateType.Message]
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await botClient.ReceiveAsync(updateHandler, ReceiverOptions, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Telegram update receiver loop crashed, restarting in 5s");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
