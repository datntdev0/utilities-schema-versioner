using datntdev.SchemaVersioner.Commands;
using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using CommandType = datntdev.SchemaVersioner.Models.CommandType;

[assembly: InternalsVisibleTo("datntdev.SchemaVersioner.Tests")]

namespace datntdev.SchemaVersioner
{
    public class SchemaVersioner(IDbConnection connection, ILogger logger, Settings settings)
    {
        public void Info()
        {
            var command = GetCommand(CommandType.Info);
            command.PrintResult(command.Execute(settings));
        }

        public void Init()
        {
            var command = GetCommand(CommandType.Init);
            command.PrintResult(command.Execute(settings));
        }

        public void Upgrade()
        {
            var command = GetCommand(CommandType.Upgrade);
            command.PrintResult(command.Execute(settings));
        }

        public void Downgrade()
        {
            var command = GetCommand(CommandType.Downgrade);
            command.PrintResult(command.Execute(settings));
        }

        public void Validate()
        {
            var command = GetCommand(CommandType.Validate);
            command.PrintResult(command.Execute(settings));
        }

        public void Repair()
        {
            var command = GetCommand(CommandType.Repair);
            command.PrintResult(command.Execute(settings));
        }

        public void Snapshot()
        {
            var command = GetCommand(CommandType.Snapshot);
            command.PrintResult(command.Execute(settings));
        }

        private ICommand GetCommand(CommandType commandType)
        {
            var connector = ConnectorFactory.CreateConnector(logger, connection);

            ICommand command = commandType switch
            {
                CommandType.Info => new InfoCommand(connector, logger),
                CommandType.Init => new InitCommand(connector, logger),
                CommandType.Upgrade => new UpgradeCommand(connector, logger),
                CommandType.Downgrade => new DowngradeCommand(connector, logger),
                CommandType.Validate => new ValidateCommand(connector, logger),
                CommandType.Repair => new RepairCommand(connector, logger),
                CommandType.Snapshot => new SnapshotCommand(connector, logger),
                _ => throw new NotSupportedException($"Command type {commandType} is not supported.")
            };

            return command;
        }
    }
}
