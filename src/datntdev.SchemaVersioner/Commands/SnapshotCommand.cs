using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class SnapshotCommand(ILogger logger) : BaseCommand(logger), ICommand
    {
        private readonly SnapshotLoader _loader = new();

        public CommandOutput Execute(Settings settings)
        {
            var snapshots = _loader.Load(settings);
            return new();
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
