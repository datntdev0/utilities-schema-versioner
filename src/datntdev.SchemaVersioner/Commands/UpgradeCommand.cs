using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
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
                .OrderBy(x => x.Version).ToList();

            // Assign latest and target versions
            var latestVersion = migrationRecords.LastOrDefault()?.Version ?? string.Empty;
            var targetVersion = string.IsNullOrEmpty(_settings.TargetVersion) ?
                migrationScripts.LastOrDefault()?.Version : _settings.TargetVersion;

            // If target version is not specified, upgrade to the latest version
            if (migrationScripts.Select(x => x.Version).Contains(targetVersion) == false)
            {
                throw new InvalidOperationException($"Target version '{targetVersion}' does not exist in migration scripts.");
            }

            // Determine migrations to run pending migrations
            _logger.LogInformation("Running pending migrations to the target version {0}", targetVersion);
            var runningMigrations = migrationScripts
                .Where(x => string.Compare(x.Version, latestVersion) > 0
                    && string.Compare(x.Version, targetVersion) <= 0)
                .ToList();

            runningMigrations.ForEach(migration =>
            {
                _logger.LogInformation("Running migration {0} - {1}", migration.Version, migration.Description);
                _baseConnector.ExecuteComplexContent(migration.Content);
                _dbEngine.InsertMigrationRecord(migration);
            });

            // Run repeatable migrations
            _logger.LogInformation("Running repeatable migrations...");
            var repeatableMigrations = scripts
                .Where(x => x.Type == MigrationType.Repeatable)
                .ToList();
            repeatableMigrations.ForEach(migration =>
            {
                _logger.LogInformation("Running repeatable migration {0} - {1}", migration.Version, migration.Description);
                _baseConnector.ExecuteComplexContent(migration.Content);
            });

            return new CommandOutput<CommandOutputUpgrade>(new CommandOutputUpgrade());
        }
    }
}
