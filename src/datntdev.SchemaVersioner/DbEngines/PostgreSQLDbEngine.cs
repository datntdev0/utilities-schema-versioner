using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System;
using System.Data;
using System.Linq;

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
            var sql = $@"
                DELETE FROM ""{_settings.MetadataSchema}"".""{_settings.MetadataTable}"" 
                WHERE version = '{version}' 
                AND type = {(int)MigrationType.Versioned};";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DropMetadataTable()
        {
            var sql = $@"DROP TABLE IF EXISTS ""{_settings.MetadataSchema}"".""{_settings.MetadataTable}"";";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void EraseDatabase()
        {
            var getTablesAndViews = $@"
                SELECT * FROM information_schema.tables
                WHERE ""table_schema"" <> 'pg_catalog' AND ""table_schema"" <> 'information_schema'";
            var dropSqls = _baseConnector.ExecuteQuery(getTablesAndViews).AsEnumerable()
                .OrderByDescending(x => x.Field<string>("table_type"))
                .Select(x => new
                {
                    type = x.Field<string>("table_type")!.Replace("BASE ", ""),
                    name = x.Field<string>("table_name"),
                    schema = x.Field<string>("table_schema"),
                })
                .Select(x => $@"DROP {x.type.ToUpper()} ""{x.schema}"".""{x.name}"";");

            if (dropSqls.Any()) _baseConnector.ExecuteNonQuery(string.Join(";", dropSqls));

            var getRoutines = $@"
                SELECT * FROM information_schema.routines
                WHERE ""specific_schema"" <> 'pg_catalog' AND ""specific_schema"" <> 'information_schema'";
            var dropRoutinesSqls = _baseConnector.ExecuteQuery(getRoutines).AsEnumerable()
                .OrderBy(x => x.Field<string>("routine_type"))
                .Select(x => new
                {
                    type = x.Field<string>("routine_type"),
                    name = x.Field<string>("routine_name"),
                    schema = x.Field<string>("specific_schema"),
                })
                .Select(x => $@"DROP {x.type!.ToUpper()} ""{x.schema}"".""{x.name}"";");

            if (dropRoutinesSqls.Any()) _baseConnector.ExecuteNonQuery(string.Join(";", dropRoutinesSqls));
        }

        public Migration[] GetMetadataTable()
        {
            var sql = $@"
                SELECT type, version, description, checksum, installed_by, installed_on, success 
                FROM ""{_settings.MetadataSchema}"".""{_settings.MetadataTable}"" 
                ORDER BY installed_on DESC;";

            var dataTable = _baseConnector.ExecuteQuery(sql);
            return dataTable.AsEnumerable().Select(row => new Migration
            {
                Type = (MigrationType)row.Field<int>("type"),
                Version = row.Field<string>("version")!,
                Description = row.Field<string>("description")!,
                Checksum = row.Field<string>("checksum")!,
                InstalledBy = row.Field<string>("installed_by")!,
                InstalledAt = row.Field<DateTime>("installed_on"),
                IsSuccessful = row.Field<bool>("success")
            }).ToArray();
        }

        public Snapshot[] GetObjectSnapshots()
        {
            throw new NotSupportedException("Snapshot feature is not supported for PostgreSQL.");
        }

        public void InsertMigrationRecord(Migration x)
        {
            var sql = $@"
                INSERT INTO ""{_settings.MetadataSchema}"".""{_settings.MetadataTable}""
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
