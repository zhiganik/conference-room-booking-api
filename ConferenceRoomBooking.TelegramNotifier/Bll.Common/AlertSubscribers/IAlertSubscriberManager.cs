namespace ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;

/// <summary>Tracks which Telegram chats should receive pushes, so a broadcast can reach every
/// chat that has ever messaged the bot instead of one fixed chat id.</summary>
public interface IAlertSubscriberManager
{
    /// <returns><see langword="true"/> if the chat wasn't already subscribed.</returns>
    Task<bool> SubscribeAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>Stops tracking a chat, e.g. once it has blocked the bot.</summary>
    Task UnsubscribeAsync(long chatId, CancellationToken cancellationToken);

    Task<IReadOnlyList<long>> GetAllAsync(CancellationToken cancellationToken);
}
