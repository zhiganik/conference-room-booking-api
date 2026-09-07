using ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ConferenceRoomBooking.TelegramNotifier.Telegram;

public class TelegramUpdateHandler(IAlertSubscriberManager alertSubscriberManager, ILogger<TelegramUpdateHandler> logger) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chat = update.Message?.Chat;

        if (chat is null)
            return;
        
        var isNewSubscriber = await alertSubscriberManager.SubscribeAsync(chat.Id, cancellationToken);

        if (!isNewSubscriber)
            return;
        

        logger.LogInformation("New push subscriber: chat {ChatId} ({ChatType})", chat.Id, chat.Type);
        await botClient.SendMessage(chat.Id, "You're now subscribed to Conference Room Booking push notifications.",
            cancellationToken: cancellationToken);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Telegram update polling error ({Source})", source);

        if (source == HandleErrorSource.PollingError)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
