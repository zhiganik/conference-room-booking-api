using ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.TelegramNotifier.Queueing;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace ConferenceRoomBooking.TelegramNotifier.BackgroundServices;

public class TelegramNotifierBackgroundService(
    AlertMessageQueue queue,
    ITelegramBotClient botClient,
    IAlertSubscriberManager alertSubscriberManager,
    ILogger<TelegramNotifierBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await BroadcastAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to broadcast a queued message");
            }
        }
    }

    private async Task BroadcastAsync(string message, CancellationToken cancellationToken)
    {
        var chatIds = await alertSubscriberManager.GetAllAsync(cancellationToken);

        await Task.WhenAll(chatIds.Select(chatId => SendAsync(chatId, message, cancellationToken)));
    }

    private async Task SendAsync(long chatId, string message, CancellationToken cancellationToken)
    {
        try
        {
            await botClient.SendMessage(chatId, message, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == (int)HttpStatusCode.Forbidden || IsChatNotFound(ex))
        {
            logger.LogInformation("Chat {ChatId} is no longer reachable, removing it", chatId);
            await alertSubscriberManager.UnsubscribeAsync(chatId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deliver a message to chat {ChatId}", chatId);
        }
    }

    private static bool IsChatNotFound(ApiRequestException ex) =>
        ex.ErrorCode == (int)HttpStatusCode.BadRequest && ex.Message.Contains("chat not found", StringComparison.OrdinalIgnoreCase);
}
