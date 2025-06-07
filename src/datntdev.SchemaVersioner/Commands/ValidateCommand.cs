using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class ValidateCommand(IConnector connector, ILogger logger) : BaseCommand(connector, logger), ICommand
    {
        private readonly MigrationLoader _loader = new();

        public CommandOutput Execute(Settings settings)
        {
            var migrations = _loader.Load(settings);
            return new(new CommandOutputValidate());
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
