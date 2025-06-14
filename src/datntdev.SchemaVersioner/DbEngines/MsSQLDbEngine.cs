using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            var getTablesAndViews = $@"SELECT * FROM INFORMATION_SCHEMA.TABLES";
            var dropSqls = _baseConnector.ExecuteQuery(getTablesAndViews).AsEnumerable()
                .OrderBy(x => x.Field<string>("TABLE_TYPE"))
                .Select(x => new 
                { 
                    type = x.Field<string>("TABLE_TYPE")!.Replace("BASE ", ""),
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
                .Select(x => $"DROP {x.type!.ToUpper()} [{x.schema}].[{x.name}]");

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
            var server = new Server(new ServerConnection(((SqlConnection)_baseConnector.DbConnection)));
            var database = server.Databases[_baseConnector.DbConnection.Database];
            server.SetDefaultInitFields(typeof(Table), "IsSystemObject");
            server.SetDefaultInitFields(typeof(View), "IsSystemObject");
            server.SetDefaultInitFields(typeof(StoredProcedure), "IsSystemObject");
            server.SetDefaultInitFields(typeof(UserDefinedFunction), "IsSystemObject");

            // Define a Scripter object and set the required scripting options.   
            var scrp = new Scripter(server);
            scrp.Options.ScriptDrops = false;

            var snapshots = new List<Snapshot>();

            _logger.LogInformation("Generating DDL for database tables...");
            var tables = database.Tables.Cast<Table>()
                .Where(t => !t.IsSystemObject && t.Name != _settings.MetadataTable)
                .Select((x, i) => GetSnapshortWithScripter(scrp, x, i + 1))
                .ToArray();
            _logger.LogInformation("Generating DDL for database views...");
            var views = database.Views.Cast<View>()
                .Where(v => !v.IsSystemObject)
                .Select((x, i) => GetSnapshortWithScripter(scrp, x, i + 1))
                .ToArray();
            _logger.LogInformation("Generating DDL for database procedures...");
            var procedures = database.StoredProcedures.Cast<StoredProcedure>()
                .Where(sp => !sp.IsSystemObject)
                .Select((x, i) => GetSnapshortWithScripter(scrp, x, i + 1))
                .ToArray();
            _logger.LogInformation("Generating DDL for database functions...");
            var functions = database.UserDefinedFunctions.Cast<UserDefinedFunction>()
                .Where(udf => !udf.IsSystemObject)
                .Select((x, i) => GetSnapshortWithScripter(scrp, x, i + 1))
                .ToArray();

            return [.. tables, .. views, .. procedures, .. functions];
        }

        private static Snapshot GetSnapshortWithScripter<TObject>(Scripter scrp, TObject schemaObject, int order)
            where TObject : ScriptSchemaObjectBase
        {
            var type = typeof(TObject) switch
            {
                Type t when t == typeof(Table) => SnapshotType.Table,
                Type t when t == typeof(View) => SnapshotType.View,
                Type t when t == typeof(StoredProcedure) => SnapshotType.Procedure,
                Type t when t == typeof(UserDefinedFunction) => SnapshotType.Function,
                _ => SnapshotType.None,
            };
            var sc = scrp.Script(new Urn[] { schemaObject.Urn });
            var content = string.Join($"\r\nGO\r\n", sc.Cast<string>());
            return new Snapshot
            {
                Type = type,
                Order = order.ToString("D3"),
                Description = schemaObject.Name,
                ContentDDL = content,
            };
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
