namespace ConferenceRoomBooking.Bll.Common.Notifications;

/// <summary>
/// Pushes a text message to a messaging channel (Telegram, WhatsApp, Viber, …). Business logic
/// depends only on this abstraction — swapping the channel means registering a different
/// implementation, nothing that calls <see cref="PushAsync"/> changes.
/// </summary>
public interface INotifier
{
    Task PushAsync(string message, CancellationToken cancellationToken);
}
