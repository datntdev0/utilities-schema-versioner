using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class DowngradeCommand(ILogger logger) : BaseCommand(logger), ICommand
    {
        private readonly MigrationLoader _loader = new();

        public CommandOutput Execute(Settings settings)
        {
            var migrations = _loader.Load(settings);
            return new();
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
