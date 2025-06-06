using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InfoCommand(ILogger logger) : BaseCommand(logger), ICommand
    {
        public CommandOutput Execute(Settings settings)
        {
            return new();
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
