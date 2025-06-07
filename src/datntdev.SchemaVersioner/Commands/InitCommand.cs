using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InitCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Load schema snapshots from snapshot folders

            // Run all schema snapshots to init new database

            // Create metadata table if not exists

            // Load migrations scripts from migration folders

            // Seed all migration records to metadata table

            return new CommandOutput<CommandOutputInit>(new CommandOutputInit());
        }
    }
}
