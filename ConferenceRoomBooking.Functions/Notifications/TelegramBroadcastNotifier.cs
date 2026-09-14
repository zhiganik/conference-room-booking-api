using System.Net;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.Bll.Common.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace ConferenceRoomBooking.Functions.Notifications;

public class TelegramBroadcastNotifier(
    IAlertSubscriberManager alertSubscriberManager,
    ITelegramBotClient botClient,
    ILogger<TelegramBroadcastNotifier> logger) : INotifier
{
    public async Task PushAsync(string message, CancellationToken cancellationToken)
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
