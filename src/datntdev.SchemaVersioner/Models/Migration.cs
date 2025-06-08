using System;
using System.IO;
using System.Linq;

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

        public string Content => File.ReadAllText(FilePath);

        public string ContentChecksum => System.Security.Cryptography.MD5.Create()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes($"{Path.GetFileName(FilePath)}\n{Content}"))
                .Aggregate(string.Empty, (current, b) => current + b.ToString("x2"));
    }
}
