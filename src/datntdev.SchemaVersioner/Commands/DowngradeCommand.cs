using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class DowngradeCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
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
            var migrationScripts = new MigrationLoader().Load(_settings)
                .Where(x => x.Type == MigrationType.Undo)
                .OrderBy(x => x.Version).ToList();

            var latestVersion = migrationRecords.LastOrDefault()?.Version ?? string.Empty;
            var targetVersion = _settings.TargetVersion;

            // If target version is not specified, downgrade the latest version
            if (string.IsNullOrEmpty(targetVersion))
            {
                _logger.LogWarning("Target version is not specified. Downgrade the latest version.");
                var undoLatestMigration = migrationScripts.FirstOrDefault(x => x.Version == latestVersion);
                if (undoLatestMigration != null)
                {
                    _logger.LogInformation("Running downgrade for the latest migration {0} - {1}",
                        undoLatestMigration.Version, undoLatestMigration.Description);
                    _baseConnector.ExecuteNonQuery(undoLatestMigration.Content);
                    _dbEngine.DeleteMigrationRecord(undoLatestMigration.Version);
                }
                else
                {
                    _logger.LogWarning("No downgrade script found for the latest migration version {0}.", latestVersion);
                }
            }
            else // If target version is specified, downgrade to that version
            {
                // If target version is not specified, upgrade to the latest version
                if (migrationScripts.Select(x => x.Version).Contains(targetVersion) == false)
                {
                    throw new InvalidOperationException($"Target version '{targetVersion}' does not exist in migration history.");
                }

                // Compare two list of versions to ensure they match to have undo scripts to downgrade
                var undoMigrations = migrationRecords
                    .Where(x => string.Compare(x.Version, targetVersion) >= 0
                        && string.Compare(x.Version, latestVersion) <= 0)
                    .ToList();
                var undoScripts = migrationScripts
                    .Where(x => string.Compare(x.Version, targetVersion) >= 0
                        && string.Compare(x.Version, latestVersion) <= 0)
                    .ToList();

                var undoMigrationVersions = undoMigrations.Select(x => x.Version).ToList();
                var undoScriptVersions = undoScripts.Select(x => x.Version).ToList();
                if (!undoMigrationVersions.SequenceEqual(undoScriptVersions))
                {
                    throw new InvalidOperationException("The list of in-downgrading migrations does not match the list of undo scripts.");
                }

                // Run pending downgrades to the target version
                _logger.LogInformation("Running pending downgrades to the target version {0}", _settings.TargetVersion);
                undoScripts.ForEach(migration =>
                {
                    _logger.LogInformation("Running downgrade {0} - {1}", migration.Version, migration.Description);
                    _baseConnector.ExecuteNonQuery(migration.Content);
                    _dbEngine.DeleteMigrationRecord(migration.Version);
                });
            }

            return new CommandOutput<CommandOutputDowngrade>(new CommandOutputDowngrade());
        }
    }
}
