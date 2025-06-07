using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InfoCommand(IConnector connector, ILogger logger) 
        : BaseCommand(connector, logger), ICommand
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
