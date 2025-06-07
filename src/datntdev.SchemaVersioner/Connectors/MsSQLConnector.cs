using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class MsSQLConnector(SchemaVersionerContext context) : BaseConnector(context), IConnector
    {
        public DbEngineType DbEngineType => DbEngineType.MsSQL;

        protected override string SQL_CheckVersion => @"
            SELECT COUNT(*)
            FROM (SELECT @@VERSION AS _VERSION) AS t
            WHERE t._VERSION LIKE '%SQL Server%';";

        protected override string SQL_GetVersion => "SELECT @@VERSION";
    }
}
