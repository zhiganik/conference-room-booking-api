using ConferenceRoomBooking.Bll.Common.Notifications;
using Telegram.Bot;

namespace ConferenceRoomBooking.Functions.Notifications;

public class TelegramWebhookRegistrar(ITelegramBotClient botClient) : IWebhookRegistrar
{
    public Task RegisterAsync(string webhookUrl, CancellationToken cancellationToken) =>
        botClient.SetWebhook(webhookUrl, cancellationToken: cancellationToken);
}
