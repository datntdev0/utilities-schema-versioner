using datntdev.SchemaVersioner.Commands;
using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

using CommandType = datntdev.SchemaVersioner.Models.CommandType;

namespace datntdev.SchemaVersioner.Helpers
{
    internal static class Factory
    {
        public static ICommand CreateCommand(CommandType commandType, SchemaVersionerContext context)
        {
            ICommand command = commandType switch
            {
                CommandType.Info => new InfoCommand(context),
                CommandType.Init => new InitCommand(context),
                CommandType.Upgrade => new UpgradeCommand(context),
                CommandType.Downgrade => new DowngradeCommand(context),
                CommandType.Validate => new ValidateCommand(context),
                CommandType.Repair => new RepairCommand(context),
                CommandType.Snapshot => new SnapshotCommand(context),
                _ => throw new NotSupportedException($"Command type {commandType} is not supported.")
            };

            return command;
        }

        public static SchemaVersionerContext CreateContext(IDbConnection dbConnection, ILogger logger, Settings settings)
        {
            ArgumentNullHelper.ThrowIfNull(dbConnection, nameof(dbConnection));
            ArgumentNullHelper.ThrowIfNull(logger, nameof(logger));
            ArgumentNullHelper.ThrowIfNull(settings, nameof(settings));

            var context = new SchemaVersionerContext()
            {
                Logger = logger,
                Settings = settings,
                DbConnection = dbConnection,
            };

            context.Connector = CreateConnector(context);
            context.DbEngine = CreateDbEngine(context);

            return context;
        }

        private static IConnector CreateConnector(SchemaVersionerContext context)
        {
            foreach (var dbEngine in Enum.GetValues(typeof(DbEngineType)))
            {
                IConnector? connector = dbEngine switch
                {
                    DbEngineType.MsSQL => new Connectors.MsSQLConnector(context),
                    DbEngineType.SQLite => new Connectors.SQLiteConnector(context),
                    _ => null,
                };

                if (connector != null && connector.IsSupported())
                {
                    context.Logger.LogInformation($"Using {dbEngine} connector for database versioning.");
                    return connector;
                }
            }
            throw new NotSupportedException($"Not found any database engine supporting your connection.");
        }

        private static IDbEngine CreateDbEngine(SchemaVersionerContext context)
        {
            return context.Connector.DbEngineType switch
            {
                DbEngineType.SQLite => new DbEngines.SQLiteDbEngine(context),
                DbEngineType.MsSQL => new DbEngines.MsSQLDbEngine(context),
                _ => throw new NotSupportedException($"Not found any database engine supporting your connection."),
            };
        }
    }
}
