using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class SnapshotCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Scan and generate DDL for all database objects
            _logger.LogInformation("Generating DDL for all database objects...");
            var snapshots = _dbEngine.GetObjectSnapshots();

            // Create sql scripts for that all DDL
            _logger.LogInformation("Writing DDL to output path: {OutputPath}", _settings.SnapshotOutputPath);
            new SnapshotLoader().Write(snapshots, _settings);

            return new CommandOutput<CommandOutputSnapshot>(new CommandOutputSnapshot());
        }
    }
}
