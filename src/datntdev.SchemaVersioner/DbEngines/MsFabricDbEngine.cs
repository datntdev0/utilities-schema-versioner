using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System;
using System.Data;
using System.Linq;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class MsFabricDbEngine : BaseDbEngine, IDbEngine
    {
        public MsFabricDbEngine(SchemaVersionerContext context) : base(context)
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
                    id BIGINT NOT NULL, 
                    type SMALLINT, 
                    version VARCHAR(50), 
                    description VARCHAR(200) NOT NULL, 
                    checksum VARCHAR(32), 
                    installed_by VARCHAR(100) NOT NULL, 
                    installed_on DATETIME2(0) NOT NULL, 
                    success BIT NOT NULL 
                );";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DeleteMigrationRecord(string version)
        {
            var sql = $@"
                DELETE FROM [{_settings.MetadataSchema}].[{_settings.MetadataTable}] 
                WHERE version = '{version}' 
                AND type = {(int)MigrationType.Versioned};";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DropMetadataTable()
        {
            var sql = $@"DROP TABLE IF EXISTS [{_settings.MetadataSchema}].[{_settings.MetadataTable}]";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void EraseDatabase()
        {
            var getTablesAndViews = $@"
                SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA <> 'sys' AND TABLE_SCHEMA <> 'queryinsights'";
            var dropSqls = _baseConnector.ExecuteQuery(getTablesAndViews).AsEnumerable()
                .OrderBy(x => x.Field<string>("TABLE_TYPE"))
                .Select(x => new 
                { 
                    type = x.Field<string>("TABLE_TYPE").Replace("BASE ", ""),
                    name = x.Field<string>("TABLE_NAME"),
                    schema = x.Field<string>("TABLE_SCHEMA")
                })
                .Select(x => $"DROP {x.type.ToUpper()} [{x.schema}].[{x.name}]");

            if (dropSqls.Any()) _baseConnector.ExecuteNonQuery(string.Join(";", dropSqls));

            var getRoutines = @"SELECT * FROM INFORMATION_SCHEMA.ROUTINES";
            var dropRoutinesSqls = _baseConnector.ExecuteQuery(getRoutines).AsEnumerable()
                .OrderBy(x => x.Field<string>("ROUTINE_TYPE"))
                .Select(x => new 
                { 
                    type = x.Field<string>("ROUTINE_TYPE"), 
                    name = x.Field<string>("ROUTINE_NAME"), 
                    schema = x.Field<string>("ROUTINE_SCHEMA") 
                })
                .Select(x => $"DROP {x.type.ToUpper()} [{x.schema}].[{x.name}]");

            if (dropRoutinesSqls.Any()) _baseConnector.ExecuteNonQuery(string.Join(";", dropRoutinesSqls));
        }

        public Migration[] GetMetadataTable()
        {
            var sql = $@"
                SELECT type, version, description, checksum, installed_by, installed_on, success 
                FROM [{_settings.MetadataSchema}].[{_settings.MetadataTable}] 
                ORDER BY installed_on DESC;";

            var dataTable = _baseConnector.ExecuteQuery(sql);
            return dataTable.AsEnumerable().Select(row => new Migration
            {
                Type = (MigrationType)row.Field<short>("type"),
                Version = row.Field<string>("version"),
                Description = row.Field<string>("description"),
                Checksum = row.Field<string>("checksum"),
                InstalledBy = row.Field<string>("installed_by"),
                InstalledAt = row.Field<DateTime>("installed_on"),
                IsSuccessful = row.Field<bool>("success")
            }).ToArray();
        }

        public Snapshot[] GetObjectSnapshots()
        {
            throw new System.NotImplementedException();
        }

        public void InsertMigrationRecord(Migration migration)
        {
            var sql = $@"
                INSERT INTO [{_settings.MetadataSchema}].[{_settings.MetadataTable}] 
                (id, type, version, description, checksum, installed_by, installed_on, success) 
                VALUES 
                (
                    CAST(FORMAT(GETDATE(), 'yyyyMMddHHmmssfff') AS BIGINT),
                    {(int)migration.Type}, 
                    '{migration.Version}',
                    '{migration.Description}', 
                    '{migration.ContentChecksum}', 
                    SUSER_SNAME(),
                    GETDATE(),
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
