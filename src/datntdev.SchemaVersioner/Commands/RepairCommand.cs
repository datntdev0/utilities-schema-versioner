using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class RepairCommand(IConnector connector, IDbEngine dbEngine, ILogger logger)
        : BaseCommand(connector, dbEngine, logger), ICommand
    {
        private readonly MigrationLoader _loader = new();

        public CommandOutput Execute(Settings settings)
        {
            var migrations = _loader.Load(settings);
            return new(new CommandOutputRepair());
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
