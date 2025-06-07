using datntdev.SchemaVersioner.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace datntdev.SchemaVersioner.Helpers
{
    internal class ConnectorFactory
    {
        public static BaseConnector CreateConnector(ILogger logger, IDbConnection dbConnection)
        {
            throw new NotSupportedException($"Not found any database engine supporting your connection.");
        }
    }
}
