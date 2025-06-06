using datntdev.SchemaVersioner.Models;
using System.IO;
using System.Linq;

namespace datntdev.SchemaVersioner.Loaders
{
    internal class SnapshotLoader
    {
        public Snapshot[] Load(Settings settings)
        {
            return settings.SnapshotPaths
                .Where(Directory.Exists)
                .SelectMany(x => Directory.GetFiles(x, "*.sql", SearchOption.AllDirectories))
                .Select(x => ParseSnapshot(Path.GetFullPath(x)))
                .ToArray();
        }

        private Snapshot ParseSnapshot(string filePath)
        {
            var prefix = Path.GetFileName(filePath).Substring(0, 1);
            var type = prefix switch
            {
                Constants.Prefixes.SnapshotSchema => SnapshotType.Schema,
                Constants.Prefixes.SnapshotTable => SnapshotType.Table,
                Constants.Prefixes.SnapshotView => SnapshotType.View,
                Constants.Prefixes.SnapshotFunction => SnapshotType.Function,
                Constants.Prefixes.SnapshotProcedure => SnapshotType.Procedure,
                _ => SnapshotType.None
            };
            var splited = Path.GetFileNameWithoutExtension(filePath).Split("__", 2);
            var description = splited[1].Replace("_", " ");
            var order = splited[0][2..];

            return new Snapshot()
            {
                Type = type,
                Order = order,
                Description = description,
                FilePath = filePath,
            };
        }
    }
}
