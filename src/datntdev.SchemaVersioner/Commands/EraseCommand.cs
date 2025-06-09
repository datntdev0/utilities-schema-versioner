using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class EraseCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            _logger.LogInformation("Erasing database...");
            _dbEngine.EraseDatabase();

            return new CommandOutput<CommandOutputErase>(new CommandOutputErase());
        }
    }
}
