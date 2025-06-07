using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InitCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        private readonly SnapshotLoader _loader = new();

        public CommandOutput Execute(Settings settings)
        {
            var snapshots = _loader.Load(settings);
            return new(new CommandOutputInit());
        }

        public void PrintResult(CommandOutput output)
        {
        }
    }
}
