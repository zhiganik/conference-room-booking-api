using Azure.Identity;
using DbUp;
using DbUp.Engine;
using DbUp.Support;
using DbUp.SqlServer;
using Microsoft.Extensions.Configuration;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Migrations;

public static class DatabaseMigrator
{
    public static void Migrate(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var assembly = typeof(DatabaseMigrator).Assembly;
        var connectionManager = new AzureSqlConnectionManager(connectionString, new DefaultAzureCredential());

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionManager, "MZhehistovskyi")
            .WithScriptsEmbeddedInAssembly(assembly, IsMigrationScript, new SqlScriptOptions { ScriptType = ScriptType.RunOnce })
            .WithScriptsEmbeddedInAssembly(assembly, IsProcedureScript, new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new InvalidOperationException("Database migration failed.", result.Error);
        }
    }

    private static bool IsMigrationScript(string resourceName) =>
        resourceName.Contains("._01_Migrations.", StringComparison.OrdinalIgnoreCase)
        && resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase);

    private static bool IsProcedureScript(string resourceName) =>
        resourceName.Contains("._02_Procedures.", StringComparison.OrdinalIgnoreCase)
        && resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase);
}
