using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using System;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InitCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        private const string MetadataTableAlreadyExistsError = "Metadata table already exists. We only initialize for new database";

        public CommandOutput Execute()
        {
            // Load schema snapshots from snapshot folders
            var snapshots = new SnapshotLoader().Load(_settings)
                .OrderBy(x => x.Type).ThenBy(x => x.Order).ToList();

            // Run all schema snapshots to init new database
            snapshots.ForEach(x => _baseConnector.ExecuteNonQuery(x.Content));

            // Create metadata table if not exists
            if (_dbEngine.IsMetadataTableExists())
            {
                throw new InvalidOperationException(MetadataTableAlreadyExistsError);
            }

            _dbEngine.CreateMetadataTable();

            // Load migrations scripts from migration folders
            var migrations = new MigrationLoader().Load(_settings)
                .Where(x => x.Type == MigrationType.Versioned)
                .OrderBy(x => x.Version).ToList();

            // Seed all migration records to metadata table
            migrations.ForEach(x => _dbEngine.InsertMigrationRecord(x));

            return new CommandOutput<CommandOutputInit>(new CommandOutputInit());
        }
    }
}
