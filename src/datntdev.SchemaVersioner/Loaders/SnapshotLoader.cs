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

        public void Write(Snapshot[] snapshots, Settings settings)
        {
            if (!Directory.Exists(settings.SnapshotOutputPath))
            {
                Directory.CreateDirectory(settings.SnapshotOutputPath);
            }

            // Create snapshot output files for Tables
            var tablesDirectory = $"{settings.SnapshotOutputPath}/Tables";
            if (!Directory.Exists(tablesDirectory)) Directory.CreateDirectory(tablesDirectory);
            snapshots.Where(x => x.Type == SnapshotType.Table)
                .ToList()
                .ForEach(x => WriteSnapshotFile(x, tablesDirectory));

            // Create snapshot output files for Views
            var viewsDirectory = $"{settings.SnapshotOutputPath}/Views";
            if (!Directory.Exists(viewsDirectory)) Directory.CreateDirectory(viewsDirectory);
            snapshots.Where(x => x.Type == SnapshotType.View)
                .ToList()
                .ForEach(x => WriteSnapshotFile(x, viewsDirectory));
        }

        private void WriteSnapshotFile(Snapshot snapshot, string outputPath)
        {
            var fileName = $"{snapshot.Type.ToString().Substring(0, 1)}__{snapshot.Order}_{snapshot.Description.Replace(" ", "_")}.sql";
            var filePath = Path.Combine(outputPath, fileName);
            File.WriteAllText(filePath, snapshot.ContentDDL);
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
