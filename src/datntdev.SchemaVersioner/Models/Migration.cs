using System;

namespace datntdev.SchemaVersioner.Models
{
    public class Migration
    {
        public MigrationType Type { get; set; } = MigrationType.None;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Checksum { get; set; } = string.Empty;
        public string InstalledBy { get; set; } = string.Empty;
        public DateTime InstalledAt { get; set; } = DateTime.UtcNow;
        public bool IsSuccessful { get; set; } = false;
    }
}
