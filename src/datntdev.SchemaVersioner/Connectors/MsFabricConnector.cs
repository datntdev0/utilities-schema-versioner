using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace datntdev.SchemaVersioner.Connectors
{
    internal class MsFabricConnector(SchemaVersionerContext context) : BaseConnector(context), IConnector
    {
        public DbEngineType DbEngineType => DbEngineType.MsFabric;

        protected override string SQL_CheckVersion => @"
            SELECT CAST(COUNT(*) AS BIGINT)
            FROM (SELECT @@VERSION AS _VERSION) AS t
            WHERE t._VERSION LIKE '%SQL Data Warehouse%';";

        protected override string SQL_GetVersion => "SELECT @@VERSION";

        public override void ExecuteComplexContent(string sql)
        {
            var splits = Regex.Split(sql, @"\bGO\b", RegexOptions.Multiline);

            splits.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToList()
                .ForEach(ExecuteNonQuery);
        }
    }
}
