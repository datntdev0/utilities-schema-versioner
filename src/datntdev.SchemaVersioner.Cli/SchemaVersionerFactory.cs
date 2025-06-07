using datntdev.SchemaVersioner.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace datntdev.SchemaVersioner.Cli
{
    internal static class SchemaVersionerFactory
    {
        public static SchemaVersioner GetSchemaVersioner(string[] args, ILogger logger)
        {
            var settings = ArgParser.GetSettingsCli(args);

            IDbConnection connection = settings.DbEngineType switch
            {
                DbEngineType.SQLite => new SqliteConnection(settings.ConnectionString),
                DbEngineType.MsSQL => new SqlConnection(settings.ConnectionString),
                _ => throw new NotSupportedException($"Database type '{settings.DbEngineType}' is not supported.")
            };

            return new(connection, logger, settings);
        }
    }
}
