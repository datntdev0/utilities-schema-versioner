using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class UpgradeCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Create metadata table if not exists
            if (!_dbEngine.IsMetadataTableExists())
            {
                _dbEngine.CreateMetadataTable();
            }

            // Load migration records from metadata table
            _logger.LogInformation("Loading migration records from metadata table...");
            var migrationRecords = _dbEngine.GetMetadataTable()
                .Where(x => x.Type == MigrationType.Versioned)
                .OrderBy(x => x.Version).ToList();

            // Load migration scripts from migration folders
            _logger.LogInformation("Loading migrations scripts from migration folders...");
            var scripts = new MigrationLoader().Load(_settings);

            var migrationScripts = scripts
                .Where(x => x.Type == MigrationType.Versioned)
                .ToList();

            // Determine migrations to run pending migrations
            _logger.LogInformation("Running pending migrations in order of version number");
            var runningMigrations = migrationScripts
                .Where(x => !migrationRecords.Any(r => r.Version == x.Version))
                .ToList();

            runningMigrations.ForEach(migration =>
            {
                _logger.LogInformation("Running migration {Version} - {Description}",
                    migration.Version, migration.Description);
                _baseConnector.ExecuteComplexContent(migration.Content);
                _dbEngine.InsertMigrationRecord(migration);
            });

            // Run repeatable migrations
            _logger.LogInformation("Running repeatable migrations...");
            var repeatableRecords = _dbEngine.GetMetadataTable()
                .Where(x => x.Type == MigrationType.Repeatable)
                .ToDictionary(x => x.Description, x => x.Checksum);
            var repeatableScripts = scripts
                .Where(x => x.Type == MigrationType.Repeatable)
                .Where(x => !repeatableRecords.ContainsKey(x.Description)
                    || repeatableRecords[x.Description] != x.ContentChecksum)
                .ToList();
            repeatableScripts.ForEach(migration =>
            {
                _logger.LogInformation("Running repeatable migration {Version} - {Description}",
                    migration.Version, migration.Description);
                _baseConnector.ExecuteComplexContent(migration.Content);
                _dbEngine.UpsertRepeatableRecord(migration);
            });

            _logger.LogInformation("Database upgraded successfully to latest version with {Count} migrations.",
                runningMigrations.Count);

            return new CommandOutput<CommandOutputUpgrade>(new CommandOutputUpgrade());
        }
    }
}
