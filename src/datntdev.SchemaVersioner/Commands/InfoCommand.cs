using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Commands
{
    internal class InfoCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand 
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
