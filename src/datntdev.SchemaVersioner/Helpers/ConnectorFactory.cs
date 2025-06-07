using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace datntdev.SchemaVersioner.Helpers
{
    internal class ConnectorFactory
    {
        public static BaseConnector CreateConnector(ILogger logger, IDbConnection dbConnection)
        {
            foreach (var dbEngine in Enum.GetValues(typeof(DbEngineType)))
            {
                BaseConnector? connector = dbEngine switch
                {
                    DbEngineType.MsSQL => new Connectors.MsSQLConnector(logger, dbConnection),
                    DbEngineType.SQLite => new Connectors.SQLiteConnector(logger, dbConnection),
                    _ => null,
                };

                if (connector != null && connector.IsSupported())
                {
                    logger.LogInformation($"Using {dbEngine} connector for database versioning.");
                    return connector;
                }
            }
            throw new NotSupportedException($"Not found any database engine supporting your connection.");
        }
    }
}
