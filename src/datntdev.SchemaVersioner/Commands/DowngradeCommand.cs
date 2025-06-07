using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class DowngradeCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Create metadata table if not exists

            // Load migration records from metadata table

            // Load migration scripts from migration folders

            // Determine and run pending downgrades to target version

            return new CommandOutput<CommandOutputDowngrade>(new CommandOutputDowngrade());
        }
    }
}
