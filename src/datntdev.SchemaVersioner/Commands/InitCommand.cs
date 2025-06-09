using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InitCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        private const string MetadataTableAlreadyExistsError = "Metadata table already exists. We only initialize for new database";

        public CommandOutput Execute()
        {
            // Create metadata table if not exists
            if (_dbEngine.IsMetadataTableExists())
            {
                throw new InvalidOperationException(MetadataTableAlreadyExistsError);
            }

            // Load schema snapshots from snapshot folders
            _logger.LogInformation("Running all database schema scripts...");
            var snapshots = new SnapshotLoader().Load(_settings)
                .OrderBy(x => x.Type).ThenBy(x => x.Order).ToList();

            // Run all schema snapshots to init new database
            snapshots.ForEach(x => _baseConnector.ExecuteNonQuery(x.Content));

            // Create metadata table for migrations
            _logger.LogInformation("Creating metadata table for migrations...");
            _dbEngine.CreateMetadataTable();

            // Load migrations scripts from migration folders
            _logger.LogInformation("Loading migrations scripts from migration folders...");
            var migrations = new MigrationLoader().Load(_settings)
                .Where(x => x.Type == MigrationType.Versioned)
                .OrderBy(x => x.Version).ToList();

            // Seed all migration records to metadata table
            migrations.ForEach(_dbEngine.InsertMigrationRecord);

            return new CommandOutput<CommandOutputInit>(new CommandOutputInit());
        }
    }
}
