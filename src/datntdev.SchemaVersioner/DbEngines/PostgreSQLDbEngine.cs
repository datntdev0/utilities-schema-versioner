using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class PostgreSQLDbEngine : BaseDbEngine, IDbEngine
    {
        public PostgreSQLDbEngine(SchemaVersionerContext context) : base(context)
        {
            if (string.IsNullOrEmpty(_settings.MetadataSchema))
            {
                _settings.MetadataSchema = "public"; // Default schema for PostgreSQL
            }
        }

        public void CreateMetadataTable()
        {
            // Ensure the schema exists
            var createSchemaSql = $@"CREATE SCHEMA IF NOT EXISTS {_settings.MetadataSchema};";
            _baseConnector.ExecuteNonQuery(createSchemaSql);

            var sql = $@"
                CREATE TABLE IF NOT EXISTS {_settings.MetadataSchema}.""{_settings.MetadataTable}""
                ( 
                    id SERIAL PRIMARY KEY, 
                    type INT NOT NULL, 
                    version VARCHAR(50) NOT NULL, 
                    description VARCHAR(200) NOT NULL, 
                    checksum VARCHAR(32), 
                    installed_by VARCHAR(100) NOT NULL, 
                    installed_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, 
                    success BOOLEAN NOT NULL 
                );";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DeleteMigrationRecord(string version)
        {
            throw new NotImplementedException();
        }

        public void DropMetadataTable()
        {
            throw new NotImplementedException();
        }

        public void EraseDatabase()
        {
            throw new NotImplementedException();
        }

        public Migration[] GetMetadataTable()
        {
            throw new NotImplementedException();
        }

        public Snapshot[] GetObjectSnapshots()
        {
            throw new NotImplementedException();
        }

        public void InsertMigrationRecord(Migration x)
        {
            var sql = $@"
                INSERT INTO {_settings.MetadataSchema}.""{_settings.MetadataTable}""
                (type, version, description, checksum, installed_by, success) 
                VALUES 
                (
                    {(int)x.Type}, 
                    '{x.Version}', 
                    '{x.Description}', 
                    '{x.ContentChecksum}', 
                    current_user, 
                    true
                );";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public bool IsMetadataTableExists()
        {
            var sql = $@"
                    SELECT COUNT(*) 
                    FROM information_schema.tables 
                    WHERE table_schema = '{_settings.MetadataSchema}' 
                    AND table_name = '{_settings.MetadataTable}';";
            return _baseConnector.ExecuteScalar<long>(sql) == 1;
        }
    }
}
