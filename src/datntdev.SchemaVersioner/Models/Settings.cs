namespace datntdev.SchemaVersioner.Models
{
    public class Settings
    {
        public string? TargetVersion { get; set; } = null;
        public string MetadataSchema { get; set; } = string.Empty;
        public string MetadataTable { get; set; } = "schema_versioner_migrations";
        public string[] MigrationPaths { get; set; } = ["Migrations"];
        public string[] SnapshotPaths { get; set; } = ["Snapshots"];

        /// <summary>
        /// Run snapshots in this list as repeatable migrations. 
        /// The function, procedure, and view snapshots are typically run as repeatable migrations.
        /// Example: ["Function", "Procedure", "View"]
        /// </summary>
        public string[] SnapshotsAsRepeatable { get; set; } = [];
        public string SnapshotOutputPath { get; set; } = "SnapshotsOutput";
    }
}
