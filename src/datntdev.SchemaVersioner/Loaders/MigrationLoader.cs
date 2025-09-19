using datntdev.SchemaVersioner.Models;
using System.IO;
using System.Linq;

namespace datntdev.SchemaVersioner.Loaders
{
    internal class MigrationLoader()
    {
        public Migration[] Load(Settings settings)
        {
            var snapshots = System.Array.Empty<Migration>();
            if (settings.SnapshotsAsRepeatable.Any())
            {
                snapshots = new SnapshotLoader().Load(settings)
                    .Where(x => settings.SnapshotsAsRepeatable.Contains(x.Type.ToString()))
                    .Select(x => new Migration()
                    {
                        Type = MigrationType.Repeatable,
                        Version = File.GetCreationTimeUtc(x.FilePath).ToString("yyyyMMdd.HHmm"),
                        Description = Path.GetFileName(x.FilePath),
                        FilePath = x.FilePath,
                    })
                    .ToArray();
            }

            return settings.MigrationPaths
                .Where(Directory.Exists)
                .SelectMany(x => Directory.GetFiles(x, "*.sql", SearchOption.AllDirectories))
                .Select(x => ParseMigration(Path.GetFullPath(x)))
                .Concat(snapshots)
                .ToArray();
        }

        private static Migration ParseMigration(string filePath)
        {
            var prefix = Path.GetFileName(filePath).Substring(0, 1);
            var type = prefix switch
            {
                Constants.Prefixes.MigrationVersioned => MigrationType.Versioned,
                Constants.Prefixes.MigrationUndo => MigrationType.Undo,
                Constants.Prefixes.MigrationRepeatable => MigrationType.Repeatable,
                _ => MigrationType.None
            };
            var splited = Path.GetFileNameWithoutExtension(filePath).Split("__", 2);
            var description = type == MigrationType.Repeatable ? 
                Path.GetFileName(filePath) :
                splited[1].Replace("_", " ");
            var version = type == MigrationType.Repeatable ?
                File.GetCreationTimeUtc(filePath).ToString("yyyyMMdd.HHmm") :
                splited[0].Replace(prefix, string.Empty).Replace("_", ".")[1..];

            return new Migration()
            {
                Type = type,
                Version = version,
                Description = description,
                FilePath = filePath,
            };
        }
    }
}
