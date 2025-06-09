namespace datntdev.SchemaVersioner.Models
{
    public class Settings
    {
        public string? TargetVersion { get; set; } = null;
        public string MetadataSchema { get; set; } = string.Empty;
        public string MetadataTable { get; set; } = "schema_versioner_migrations";
        public string[] MigrationPaths { get; set; } = ["Migrations"];
        public string[] SnapshotPaths { get; set; } = ["Snapshots"];
        public string SnapshotOutputPath { get; set; } = "SnapshotsOutput";
    }
}
