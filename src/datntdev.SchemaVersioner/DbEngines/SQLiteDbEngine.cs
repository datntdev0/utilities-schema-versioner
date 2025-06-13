using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;
using System;
using System.Data;
using System.Linq;

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

        public void DeleteMigrationRecord(string version)
        {
            var sql = $@"
                DELETE FROM {_settings.MetadataTable} 
                WHERE version = '{version}' 
                AND type = {(int)MigrationType.Versioned};";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void DropMetadataTable()
        {
            var sql = @$"DROP TABLE IF EXISTS [{_settings.MetadataTable}]";
            _baseConnector.ExecuteNonQuery(sql);
        }

        public void EraseDatabase()
        {
            // Drop all views in the database
            var getTablesAndViews = @"
                SELECT name, type FROM sqlite_master 
                WHERE (type = 'view' OR type = 'table') AND name NOT LIKE 'sqlite_%';";

            var dropSqls = _baseConnector.ExecuteQuery(getTablesAndViews).AsEnumerable()
                .OrderBy(x => x.Field<string>("type"))
                .Select(x => new { type = x.Field<string>("type"), name = x.Field<string>("name") })
                .Select(x => $"DROP {x.type.ToUpper()} IF EXISTS [{x.name}]");

            if (dropSqls.Any()) _baseConnector.ExecuteNonQuery(string.Join(";", dropSqls));
        }

        public Migration[] GetMetadataTable()
        {
            var sql = $@"
                SELECT type, version, description, checksum, installed_by, installed_on, success 
                FROM {_settings.MetadataTable} 
                ORDER BY id ASC;";

            var dataTable = _baseConnector.ExecuteQuery(sql);
            return dataTable.AsEnumerable().Select(row => new Migration
            {
                Type = (MigrationType)row.Field<long>("type"),
                Version = row.Field<string>("version"),
                Description = row.Field<string>("description"),
                Checksum = row.Field<string>("checksum"),
                InstalledBy = row.Field<string>("installed_by"),
                InstalledAt = DateTime.Parse(row.Field<string>("installed_on")),
                IsSuccessful = row.Field<long>("success") == 1,
            }).ToArray();
        }

        public Snapshot[] GetObjectSnapshots()
        {
            // get table definitions for sqlite
            var sql = $@"
                SELECT name, sql FROM sqlite_master 
                WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name <> '{_settings.MetadataTable}';";

            var dataTable = _baseConnector.ExecuteQuery(sql);
            var tables = dataTable.AsEnumerable().Select((row, i) => new Snapshot
            {
                Type = SnapshotType.Table,
                Order = (i + 1).ToString("D3"),
                Description = row.Field<string>("name"),
                ContentDDL = row.Field<string>("sql")
            }).ToArray();

            sql = @"
                SELECT name, sql FROM sqlite_master 
                WHERE type='view' AND name NOT LIKE 'sqlite_%';";
            
            dataTable = _baseConnector.ExecuteQuery(sql);
            var views = dataTable.AsEnumerable().Select((row, i) => new Snapshot
            {
                Type = SnapshotType.View,
                Order = (i + 1).ToString("D3"),
                Description = row.Field<string>("name"),
                ContentDDL = row.Field<string>("sql")
            }).ToArray();

            return [.. tables, .. views];
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
                    '{migration.ContentChecksum}', 
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
