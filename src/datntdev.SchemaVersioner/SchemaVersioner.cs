using datntdev.SchemaVersioner.Commands;
using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("datntdev.SchemaVersioner.Tests")]

namespace datntdev.SchemaVersioner
{
    public class SchemaVersioner(ILogger logger, Settings settings)
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


        private ICommand GetCommand(CommandType commandType) => commandType switch
        {
            CommandType.Info => new InfoCommand(logger),
            CommandType.Init => new InitCommand(logger),
            CommandType.Upgrade => new UpgradeCommand(logger),
            CommandType.Downgrade => new DowngradeCommand(logger),
            CommandType.Validate => new ValidateCommand(logger),
            CommandType.Repair => new RepairCommand(logger),
            CommandType.Snapshot => new SnapshotCommand(logger),
            _ => throw new NotSupportedException($"Command type {commandType} is not supported.")
        };
    }
}
