using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ConferenceRoomBooking.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.Functions.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConferenceRoomBooking.Functions.Functions;

public class WebhookFunction(
    IAlertSubscriberManager alertSubscriberManager,
    ITelegramBotClient botClient,
    IOptions<TelegramSettings> settings,
    ILogger<WebhookFunction> logger)
{
    private const string StartCommand = "/start";
    private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

    [Function("WebhookFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhook")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (!IsSecretTokenValid(req))
        {
            return new UnauthorizedResult();
        }

        Update? update;
        try
        {
            update = await JsonSerializer
                .DeserializeAsync<Update>(req.Body, JsonBotAPI.Options, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            return new BadRequestResult();
        }

        var chat = update?.Message?.Chat;
        if (chat is null || !IsStartCommand(update?.Message?.Text))
        {
            return new OkResult();
        }

        await HandleSubscribeAsync(chat, cancellationToken).ConfigureAwait(false);

        return new OkResult();
    }

    private bool IsSecretTokenValid(HttpRequest req)
    {
        if (!req.Headers.TryGetValue(SecretTokenHeader, out var secret))
        {
            return false;
        }

        var expected = Encoding.UTF8.GetBytes(settings.Value.WebhookSecret);
        var actual = Encoding.UTF8.GetBytes(secret.ToString());

        return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private async Task HandleSubscribeAsync(Chat chat, CancellationToken cancellationToken)
    {
        var isNewSubscriber = await alertSubscriberManager
            .SubscribeAsync(chat.Id, cancellationToken)
            .ConfigureAwait(false);

        logger.LogInformation(
            "Processed {Status} subscriber request",
            isNewSubscriber ? "new" : "existing");

        var replyText = isNewSubscriber
            ? "You're now subscribed to Conference Room Booking push notifications."
            : "You're already subscribed to Conference Room Booking push notifications.";

        await botClient.SendMessage(chat.Id, replyText, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static bool IsStartCommand(string? text) =>
        text is not null && text.Split('@', 2)[0].Equals(StartCommand, StringComparison.OrdinalIgnoreCase);
}
