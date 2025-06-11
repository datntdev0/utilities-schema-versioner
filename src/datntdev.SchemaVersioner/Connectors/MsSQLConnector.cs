using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class MsSQLConnector(SchemaVersionerContext context) : BaseConnector(context), IConnector
    {
        public DbEngineType DbEngineType => DbEngineType.MsSQL;

        protected override string SQL_CheckVersion => @"
            SELECT CAST(COUNT(*) AS BIGINT)
            FROM (SELECT @@VERSION AS _VERSION) AS t
            WHERE t._VERSION LIKE '%SQL Server%';";

        protected override string SQL_GetVersion => "SELECT @@VERSION";

        public override void ExecuteComplexContent(string sql)
        {
            var splites = Regex.Split(sql, @"\bGO\b", RegexOptions.Multiline);

            splites.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToList()
                .ForEach(ExecuteNonQuery);
        }
    }
}
