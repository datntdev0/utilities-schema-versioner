using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InfoCommand(IConnector connector, IDbEngine dbEngine, ILogger logger) 
        : BaseCommand(connector, dbEngine, logger), ICommand
    {
        public CommandOutput Execute(Settings settings)
        {
            return new(new CommandOutputInfo());
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
