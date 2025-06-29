using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class PostgreSQLConnector(SchemaVersionerContext context) : BaseConnector(context), IConnector
    {
        public DbEngineType DbEngineType => DbEngineType.PostgreSQL;

        protected override string SQL_CheckVersion => @"
            SELECT CAST(COUNT(*) AS BIGINT)
            FROM (SELECT version() AS _VERSION) AS t
            WHERE t._VERSION LIKE '%PostgreSQL%';";

        protected override string SQL_GetVersion => "SELECT version();";
    }
}
