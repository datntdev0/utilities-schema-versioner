using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class RepairCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Recreate metadata table
            _logger.LogInformation("Recreating metadata table for migrations...");
            if (_dbEngine.IsMetadataTableExists())
            {
                _dbEngine.DropMetadataTable();
            }
            _dbEngine.CreateMetadataTable();

            // Load migration scripts from migration folders
            _logger.LogInformation("Loading migrations scripts from migration folders...");
            var migrations = new MigrationLoader().Load(_settings)
                .Where(x => x.Type == MigrationType.Versioned)
                .OrderBy(x => x.Version).ToList();

            // Seed all migration records to metadata table
            migrations.ForEach(_dbEngine.InsertMigrationRecord);

            return new CommandOutput<CommandOutputRepair>(new CommandOutputRepair());
        }
    }
}
