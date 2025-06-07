using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class SQLiteDbEngine(SchemaVersionerContext context) : BaseDbEngine(context), IDbEngine
    {
        public void CreateMetadataTable()
        {
            var sql = @$"
                CREATE TABLE [{_settings.MetadataTable}] 
                ( 
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                    type INTEGER, 
                    version VARCHAR(50), 
                    description VARCHAR(200) NOT NULL, 
                    checksum VARCHAR(32), 
                    installed_by VARCHAR(100) NOT NULL, 
                    installed_on TEXT NOT NULL DEFAULT (strftime('%Y-%m-%d %H:%M:%f','now')), 
                    success BOOLEAN NOT NULL 
                )";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void InsertMigrationRecord(Migration migration)
        {
            var sql = $@"
                INSERT INTO {_settings.MetadataTable} 
                (type, version, description, checksum, installed_by, success) 
                VALUES 
                (
                    {(int)migration.Type}, 
                    '{migration.Version}', 
                    '{migration.Description}', 
                    '{migration.Checksum}', 
                    '', 
                    1
                );";
            _baseConnector.ExecuteNonQuery(sql);
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
