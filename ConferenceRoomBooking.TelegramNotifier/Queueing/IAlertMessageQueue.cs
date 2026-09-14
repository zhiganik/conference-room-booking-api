namespace ConferenceRoomBooking.TelegramNotifier.Queueing;

/// <summary>Hand-off point between the controller and the background sender: enqueuing here
/// returns immediately, so a request thread never blocks on Telegram delivery.</summary>
public interface IAlertMessageQueue
{
    ValueTask EnqueueAsync(string message, CancellationToken cancellationToken);
}
