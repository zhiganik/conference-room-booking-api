using System.Text.Json;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.Bll.Common.Notifications;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConferenceRoomBooking.Functions.Notifications;

public class TelegramWebhookSubscriptionHandler(
    IAlertSubscriberManager alertSubscriberManager,
    ITelegramBotClient botClient,
    ILogger<TelegramWebhookSubscriptionHandler> logger) : IWebhookSubscriptionHandler
{
    public async Task HandleAsync(Stream requestBody, CancellationToken cancellationToken)
    {
        var update = await JsonSerializer.DeserializeAsync<Update>(requestBody, cancellationToken: cancellationToken);
        var chat = update?.Message?.Chat;

        if (chat is null)
            return;

        var isNewSubscriber = await alertSubscriberManager.SubscribeAsync(chat.Id, cancellationToken);

        if (!isNewSubscriber)
            return;

        logger.LogInformation("New push subscriber: chat {ChatId} ({ChatType})", chat.Id, chat.Type);
        await botClient.SendMessage(chat.Id, "You're now subscribed to Conference Room Booking push notifications.",
            cancellationToken: cancellationToken);
    }
}
