namespace datntdev.SchemaVersioner.Models
{
    internal class Snapshot
    {
        public SnapshotType Type { get; set; } = SnapshotType.None;
        public string Order { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
