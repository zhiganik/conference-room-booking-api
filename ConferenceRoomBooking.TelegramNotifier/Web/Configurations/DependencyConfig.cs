using ConferenceRoomBooking.TelegramNotifier.BackgroundServices;
using ConferenceRoomBooking.TelegramNotifier.Bll.AlertSubscribers;
using ConferenceRoomBooking.TelegramNotifier.Bll.Common.AlertSubscribers;
using ConferenceRoomBooking.TelegramNotifier.Dal.AlertSubscribers;
using ConferenceRoomBooking.TelegramNotifier.Queueing;
using ConferenceRoomBooking.TelegramNotifier.Telegram;
using ConferenceRoomBooking.TelegramNotifier.Web.Middleware;
using ConferenceRoomBooking.Utils.Sql;
using FluentValidation;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Telegram.Bot;

namespace ConferenceRoomBooking.TelegramNotifier.Web.Configurations;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        return services
            .AddExceptionHandler()
            .AddValidation()
            .AddAlertSubscribers()
            .AddTelegramClient(config)
            .AddAlertMessageQueue()
            .AddSwaggerDocs();
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddAlertSubscribers(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<IAlertSubscriberRepository, AlertSubscriberRepository>();
        services.AddSingleton<IAlertSubscriberManager, AlertSubscriberManager>();
        return services;
    }

    private static IServiceCollection AddTelegramClient(this IServiceCollection services, IConfiguration config)
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

        services.AddSingleton<TelegramUpdateHandler>();
        services.AddHostedService<TelegramReceiverBackgroundService>();

        return services;
    }

    private static IServiceCollection AddAlertMessageQueue(this IServiceCollection services)
    {
        services.AddSingleton<AlertMessageQueue>();
        services.AddSingleton<IAlertMessageQueue>(sp => sp.GetRequiredService<AlertMessageQueue>());
        services.AddHostedService<TelegramNotifierBackgroundService>();

        return services;
    }

    private static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Conference Room Booking — Telegram Notifier",
                Version = "v1",
                Description = "Internal microservice that broadcasts text messages to every Telegram chat " +
                              "that has messaged the bot. Not exposed publicly — reachable only from other " +
                              "containers on the Docker network."
            });
        });

        return services;
    }
}
