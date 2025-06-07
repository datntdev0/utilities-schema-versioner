using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class SQLiteDbEngine(SchemaVersionerContext context) : BaseDbEngine(context), IDbEngine
    {
        public void CreateMetadataTable()
        {
            throw new System.NotImplementedException();
        }

        public bool IsMetadataTableExists()
        {
            var sql = @$"
                SELECT COUNT(*) 
                FROM sqlite_master 
                WHERE type='table' AND tbl_name='{_settings.MetadataTable}';";
            return _baseConnector.ExecuteScalar<long>(sql) == 1;
        }
    }
}
