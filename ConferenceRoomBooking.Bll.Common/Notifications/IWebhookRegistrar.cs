namespace ConferenceRoomBooking.Bll.Common.Notifications;

/// <summary>
/// Tells a messaging channel (Telegram, WhatsApp, Viber, …) where to deliver its webhook calls.
/// Swapping the channel means registering a different implementation, whoever calls
/// <see cref="RegisterAsync"/> at startup doesn't change.
/// </summary>
public interface IWebhookRegistrar
{
    Task RegisterAsync(string webhookUrl, CancellationToken cancellationToken);
}
