using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class SQLiteConnector(SchemaVersionerContext context) : BaseConnector(context), IConnector
    {
        public DbEngineType DbEngineType => DbEngineType.SQLite;

        protected override string SQL_CheckVersion => @"
            SELECT COUNT(*) FROM (SELECT sqlite_version() AS _VERSION) AS t";

        protected override string SQL_GetVersion => "SELECT sqlite_version()";
    }
}
