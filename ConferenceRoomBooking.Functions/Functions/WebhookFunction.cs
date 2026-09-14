using System.Text.Json;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConferenceRoomBooking.Functions.Functions;

public class WebhookFunction(
    IAlertSubscriberManager alertSubscriberManager,
    ITelegramBotClient botClient,
    ILogger<WebhookFunction> logger)
{
    private const string StartCommand = "/start";

    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhook")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var update = await JsonSerializer.DeserializeAsync<Update>(req.Body, JsonBotAPI.Options, cancellationToken);
        var message = update?.Message;
        var chat = message?.Chat;

        if (chat is null || !IsStartCommand(message!.Text))
            return new OkResult();

        var isNewSubscriber = await alertSubscriberManager.SubscribeAsync(chat.Id, cancellationToken);

        logger.LogInformation("{Status} subscriber: chat {ChatId} ({ChatType})",
            isNewSubscriber ? "New" : "Already", chat.Id, chat.Type);

        var replyText = isNewSubscriber
            ? $"You're now subscribed to Conference Room Booking push notifications. (chat id: {chat.Id})"
            : $"You're already subscribed to Conference Room Booking push notifications. (chat id: {chat.Id})";

        await botClient.SendMessage(chat.Id, replyText, cancellationToken: cancellationToken);

        return new OkResult();
    }

    private static bool IsStartCommand(string? text) =>
        text is not null && text.Split('@', 2)[0].Equals(StartCommand, StringComparison.OrdinalIgnoreCase);
}
