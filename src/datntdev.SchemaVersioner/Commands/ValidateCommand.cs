using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace datntdev.SchemaVersioner.Commands
{
    internal class ValidateCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        private const string MetadataTableMissingMessage = "Metadata table does not exist. Please run 'init', 'repair', or 'upgrade' command first.";

        public CommandOutput Execute()
        {
            // Check metadata table is exist or return error
            if (!_dbEngine.IsMetadataTableExists())
            {
                throw new InvalidOperationException(MetadataTableMissingMessage);
            }

            // Load migration records from metadata table
            _logger.LogInformation("Loading migration records from metadata table...");
            var migrationRecords = _dbEngine.GetMetadataTable();

            // Load migration scripts from migration folders
            var migrationScripts = new MigrationLoader().Load(_settings)
                .Where(x => x.Type == MigrationType.Versioned)
                .OrderBy(x => x.Version).ToList();

            // Compare migration history with existing migration scripts
            var consoleTable = new ConsoleTables.ConsoleTable("Version", "Description", "Checksum", "Installed By", "Installed At", "Success");
            foreach (var script in migrationScripts)
            {
                var record = migrationRecords.FirstOrDefault(x => x.Version == script.Version);
                if (record == null)
                {
                    consoleTable.AddRow(script.Version, script.Description, "N/A", "N/A", "N/A", "N/A");
                }
                else if (record.Checksum == script.ContentChecksum)
                {
                    consoleTable.AddRow(record.Version, record.Description, record.Checksum, record.InstalledBy, record.InstalledAt, record.IsSuccessful ? "Yes" : "No");
                }
                else if (record.Checksum != script.ContentChecksum)
                {
                    consoleTable.AddRow(script.Version, script.Description, "Checksum Mismatch", record?.InstalledBy, record.InstalledAt, "N/A");
                }
            }
            var tableOutput = consoleTable.ToString().Replace("\r", "");
            _logger.LogInformation("Comparing migration table with migration folders...\n{0}", tableOutput);

            return new CommandOutput<CommandOutputValidate>(new CommandOutputValidate()
            {
                ResultTable = consoleTable,
            });
        }
    }
}
