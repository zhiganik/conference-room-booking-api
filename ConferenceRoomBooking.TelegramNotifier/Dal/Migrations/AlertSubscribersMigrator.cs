using ConferenceRoomBooking.TelegramNotifier.Dal.Shared;
using Microsoft.Extensions.Configuration;
using UtilsMigrator = ConferenceRoomBooking.Utils.Migrations.DatabaseMigrator;

namespace ConferenceRoomBooking.TelegramNotifier.Dal.Migrations;

public static class AlertSubscribersMigrator
{
    public static void Migrate(IConfiguration configuration) =>
        UtilsMigrator.Migrate(configuration, DbSchema.Name, typeof(DbSchema).Assembly);
}
