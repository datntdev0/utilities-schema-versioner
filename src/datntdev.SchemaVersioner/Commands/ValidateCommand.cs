using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class ValidateCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Check metadata table is exist or return error

            // Load migration records from metadata table

            // Load migration scripts from migration folders

            // Compare migration history with existing migration scripts

            return new CommandOutput<CommandOutputValidate>(new CommandOutputValidate());
        }
    }
}
