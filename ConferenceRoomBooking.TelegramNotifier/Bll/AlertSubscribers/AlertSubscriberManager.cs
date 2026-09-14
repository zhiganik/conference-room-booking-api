using ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;

namespace ConferenceRoomBooking.TelegramNotifier.Bll.AlertSubscribers;

public class AlertSubscriberManager(IAlertSubscriberRepository alertSubscriberRepository) : IAlertSubscriberManager
{
    public Task<bool> SubscribeAsync(long chatId, CancellationToken cancellationToken) =>
        alertSubscriberRepository.AddAsync(chatId, cancellationToken);

    public Task UnsubscribeAsync(long chatId, CancellationToken cancellationToken) =>
        alertSubscriberRepository.RemoveAsync(chatId, cancellationToken);

    public Task<IReadOnlyList<long>> GetAllAsync(CancellationToken cancellationToken) =>
        alertSubscriberRepository.GetAllAsync(cancellationToken);
}
