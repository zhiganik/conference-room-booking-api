namespace ConferenceRoomBooking.Bll.Common.Notifications;

/// <summary>
/// Handles an inbound webhook call from a messaging channel (Telegram, WhatsApp, Viber, …) and
/// subscribes the sender to <see cref="AlertSubscribers.IAlertSubscriberManager"/>. The HTTP
/// layer stays channel-agnostic — swapping the channel means registering a different
/// implementation, the function that calls <see cref="HandleAsync"/> doesn't change.
/// </summary>
public interface IWebhookSubscriptionHandler
{
    Task HandleAsync(Stream requestBody, CancellationToken cancellationToken);
}
