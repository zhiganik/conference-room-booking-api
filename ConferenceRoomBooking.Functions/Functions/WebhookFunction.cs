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
    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhook")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(req.Body);
        var body = await reader.ReadToEndAsync(cancellationToken);
        logger.LogInformation("Webhook payload received: {Body}", body);

        var update = JsonSerializer.Deserialize<Update>(body, JsonBotAPI.Options);
        var chat = update?.Message?.Chat;

        logger.LogInformation("Parsed update: UpdateId={UpdateId}, HasMessage={HasMessage}, ChatId={ChatId}",
            update?.Id, update?.Message is not null, chat?.Id);

        if (chat is null)
            return new OkResult();

        var isNewSubscriber = await alertSubscriberManager.SubscribeAsync(chat.Id, cancellationToken);

        if (!isNewSubscriber)
        {
            logger.LogInformation("Already subscribed: chat {ChatId} ({ChatType})", chat.Id, chat.Type);
            await botClient.SendMessage(chat.Id,
                $"You're already subscribed to Conference Room Booking push notifications. (chat id: {chat.Id})",
                cancellationToken: cancellationToken);
            return new OkResult();
        }

        logger.LogInformation("New push subscriber: chat {ChatId} ({ChatType})", chat.Id, chat.Type);
        await botClient.SendMessage(chat.Id,
            $"You're now subscribed to Conference Room Booking push notifications. (chat id: {chat.Id})",
            cancellationToken: cancellationToken);

        return new OkResult();
    }
}
