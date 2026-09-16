using System.Net;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.Bll.Common.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace ConferenceRoomBooking.Functions.Notifications;

public class TelegramBroadcastNotifier(
    IAlertSubscriberManager alertSubscriberManager,
    ITelegramBotClient botClient,
    ILogger<TelegramBroadcastNotifier> logger) : INotifier
{
    public async Task PushAsync(string message, CancellationToken cancellationToken)
    {
        var chatIds = await alertSubscriberManager.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var tasks = chatIds.Select(chatId => SendAsync(chatId, message, cancellationToken)).ToList();

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch when (tasks.Any(t => t.IsFaulted))
        {
            var failures = tasks
                .Where(t => t.IsFaulted)
                .Select(t => t.Exception!.InnerException!)
                .ToList();

            throw new AggregateException(
                $"{failures.Count}/{tasks.Count} Telegram notifications failed",
                failures);
        }
    }

    private async Task SendAsync(long chatId, string message, CancellationToken cancellationToken)
    {
        try
        {
            await botClient
                .SendMessage(chatId, message, parseMode: ParseMode.Html, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == (int)HttpStatusCode.Forbidden || IsChatNotFound(ex))
        {
            logger.LogInformation("Chat {ChatId} is no longer reachable, removing it", chatId);
            await alertSubscriberManager.UnsubscribeAsync(chatId, cancellationToken).ConfigureAwait(false);
        }
        catch (ApiRequestException ex)
        {
            logger.LogWarning(ex, "Failed to deliver a message to chat {ChatId}", chatId);
            throw;
        }
    }

    private static bool IsChatNotFound(ApiRequestException ex) =>
        ex.ErrorCode == (int)HttpStatusCode.BadRequest && ex.Message.Contains("chat not found", StringComparison.OrdinalIgnoreCase);
}
