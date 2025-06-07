using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class RepairCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Create metadata table if not exists

            // Delete all migration records from metadata table

            // Load migration scripts from migration folders

            // Seed all migration records to metadata table

            return new CommandOutput<CommandOutputRepair>(new CommandOutputRepair());
        }
    }
}
