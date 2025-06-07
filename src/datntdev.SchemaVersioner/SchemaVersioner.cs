using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Runtime.CompilerServices;
using CommandType = datntdev.SchemaVersioner.Models.CommandType;

[assembly: InternalsVisibleTo("datntdev.SchemaVersioner.Tests")]

namespace datntdev.SchemaVersioner
{
    public class SchemaVersioner(IDbConnection connection, ILogger logger, Settings settings)
    {
        private readonly SchemaVersionerContext _context = Factory.CreateContext(connection, logger, settings);

        public CommandOutput<CommandOutputInit> Init() => ExecuteCommand<CommandOutputInit>(CommandType.Init);
        public CommandOutput<CommandOutputUpgrade> Upgrade() => ExecuteCommand<CommandOutputUpgrade>(CommandType.Upgrade);
        public CommandOutput<CommandOutputDowngrade> Downgrade() => ExecuteCommand<CommandOutputDowngrade>(CommandType.Downgrade);
        public CommandOutput<CommandOutputValidate> Validate() => ExecuteCommand<CommandOutputValidate>(CommandType.Validate);
        public CommandOutput<CommandOutputRepair> Repair() => ExecuteCommand<CommandOutputRepair>(CommandType.Repair);
        public CommandOutput<CommandOutputErase> Erase() => ExecuteCommand<CommandOutputErase>(CommandType.Erase);
        public CommandOutput<CommandOutputSnapshot> Snapshot() => ExecuteCommand<CommandOutputSnapshot>(CommandType.Snapshot);

        private CommandOutput<TOutput> ExecuteCommand<TOutput>(CommandType commandType) where TOutput : class
            => (CommandOutput<TOutput>)Factory.CreateCommand(commandType, _context).Execute();
    }
}
