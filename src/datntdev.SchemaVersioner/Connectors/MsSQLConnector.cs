using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class MsSQLConnector(ILogger logger, IDbConnection dbConnection)
        : BaseConnector(logger, dbConnection)
    {
        protected override DbEngineType DbEngineType => DbEngineType.MsSQL;

        protected override string SQL_CheckVersion => @"
            SELECT COUNT(*)
            FROM (SELECT @@VERSION AS _VERSION) AS t
            WHERE t._VERSION LIKE '%SQL Server%';";

        protected override string SQL_GetVersion => "SELECT @@VERSION";
    }
}
