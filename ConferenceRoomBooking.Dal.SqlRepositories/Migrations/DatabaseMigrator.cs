using Azure.Identity;
using DbUp;
using DbUp.Engine;
using DbUp.Support;
using DbUp.SqlServer;
using Microsoft.Extensions.Configuration;

namespace ConferenceRoomBooking.Dal.SqlRepositories.Migrations;

// Forward-only, journal-tracked migrations (DbUp) instead of SSDT's state-compare Publish — the AAD
// identity used against Azure SQL has plain CREATE/ALTER rights but not VIEW DEFINITION, which VS
// Publish/SqlPackage need to reverse-engineer the target database before diffing.
//
// Two script groups, run in filename order within each:
//   01_Migrations/ — structural changes (tables, indexes, seed data). RunOnce: recorded in DbUp's
//     journal table and never re-run. Append-only — never edit a script that already ran, add a new
//     numbered one instead.
//   02_Procedures/ — stored procedures. RunAlways: re-executed on every startup, so each one must be
//     CREATE OR ALTER (idempotent) rather than plain CREATE — that's what makes editing a proc in
//     place safe, unlike a migration script.
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
