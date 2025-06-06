namespace datntdev.SchemaVersioner.Models
{
    public class Settings
    {
        public string MigrationTable { get; set; } = "schema_versioner_migrations";
        public string[] MigrationPaths { get; set; } = ["Migrations"];
        public string[] SnapshotPaths { get; set; } = ["Snapshots"];
    }
}
