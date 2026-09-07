using ConferenceRoomBooking.Dal.SqlRepositories.Shared;
using Microsoft.Extensions.Configuration;
using UtilsMigrator = ConferenceRoomBooking.Utils.Migrations.DatabaseMigrator;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Migrations;

public static class DatabaseMigrator
{
    public static void Migrate(IConfiguration configuration) =>
        UtilsMigrator.Migrate(configuration, DbSchema.Name, typeof(DbSchema).Assembly);
}
