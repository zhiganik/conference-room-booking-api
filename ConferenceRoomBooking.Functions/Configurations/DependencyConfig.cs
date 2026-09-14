using ConferenceRoomBooking.Bll.Common.Notifications;
using ConferenceRoomBooking.Functions.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace ConferenceRoomBooking.Functions.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddTelegramChannel(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TelegramSettings>(config.GetSection(TelegramSettings.SectionName));

        var telegramSettings = config.GetSection(TelegramSettings.SectionName).Get<TelegramSettings>()
                                ?? throw new InvalidOperationException("Telegram configuration section is missing.");

        if (string.IsNullOrWhiteSpace(telegramSettings.BotToken))
        {
            throw new InvalidOperationException("Telegram:BotToken is not configured.");
        }

        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, _) =>
                new TelegramBotClient(new TelegramBotClientOptions(telegramSettings.BotToken), httpClient));

        services.AddScoped<INotifier, TelegramBroadcastNotifier>();

        return services;
    }
}
