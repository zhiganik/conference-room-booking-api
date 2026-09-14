namespace ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;

/// <summary>Persistence for the Telegram chats that have messaged the push bot.</summary>
public interface IAlertSubscriberRepository
{
    /// <returns><see langword="true"/> if the chat wasn't already known.</returns>
    Task<bool> AddAsync(long chatId, CancellationToken cancellationToken);

    Task RemoveAsync(long chatId, CancellationToken cancellationToken);

    Task<IReadOnlyList<long>> GetAllAsync(CancellationToken cancellationToken);
}
