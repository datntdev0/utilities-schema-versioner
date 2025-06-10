using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class MsSQLDbEngine : BaseDbEngine, IDbEngine
    {
        public MsSQLDbEngine(SchemaVersionerContext context) : base(context)
        {
            if (string.IsNullOrEmpty(_settings.MetadataSchema))
            {
                _settings.MetadataSchema = "dbo"; // Default schema for MsSQL
            }
        }

        public void CreateMetadataTable()
        {
            // Ensure the schema exists
            var createSchemaSql = $@"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{_settings.MetadataSchema}')
                BEGIN
                    EXEC('CREATE SCHEMA [{_settings.MetadataSchema}]');
                END;";
            _baseConnector.ExecuteNonQuery(createSchemaSql);

            var sql = $@"
                CREATE TABLE [{_settings.MetadataSchema}].[{_settings.MetadataTable}] 
                ( 
                    id INT IDENTITY(1,1) PRIMARY KEY NOT NULL, 
                    type INT, 
                    version NVARCHAR(50), 
                    description NVARCHAR(200) NOT NULL, 
                    checksum NVARCHAR(32), 
                    installed_by NVARCHAR(100) NOT NULL, 
                    installed_on DATETIME NOT NULL DEFAULT GETDATE(), 
                    success BIT NOT NULL 
                );";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DeleteMigrationRecord(string version)
        {
            throw new System.NotImplementedException();
        }

        public void DropMetadataTable()
        {
            throw new System.NotImplementedException();
        }

        public void EraseDatabase()
        {
            throw new System.NotImplementedException();
        }

        public Migration[] GetMetadataTable()
        {
            throw new System.NotImplementedException();
        }

        public Snapshot[] GetObjectSnapshots()
        {
            throw new System.NotImplementedException();
        }

        public void InsertMigrationRecord(Migration migration)
        {
            var sql = $@"
                INSERT INTO [{_settings.MetadataSchema}].[{_settings.MetadataTable}] 
                (type, version, description, checksum, installed_by, success) 
                VALUES 
                (
                    {(int)migration.Type}, 
                    '{migration.Version}',
                    '{migration.Description}', 
                    '{migration.ContentChecksum}', 
                    SUSER_SNAME(),
                    1
                );";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public bool IsMetadataTableExists()
        {
            var sql = $@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = '{_settings.MetadataSchema}' AND TABLE_NAME = '{_settings.MetadataTable}';";
            return _baseConnector.ExecuteScalar<int>(sql) == 1;
        }
    }
}
