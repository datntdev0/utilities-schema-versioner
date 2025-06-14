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
            if (Directory.Exists(settings.SnapshotOutputPath))
                Directory.Delete(settings.SnapshotOutputPath, true);

            var tablesDirectory = Path.Combine(settings.SnapshotOutputPath, "Tables");
            var viewsDirectory = Path.Combine(settings.SnapshotOutputPath, "Views");
            var proceduresDirectory = Path.Combine(settings.SnapshotOutputPath, "Procedures");
            var functionsDirectory = Path.Combine(settings.SnapshotOutputPath, "Functions");

            Directory.CreateDirectory(settings.SnapshotOutputPath);
            Directory.CreateDirectory(tablesDirectory);
            Directory.CreateDirectory(viewsDirectory);
            Directory.CreateDirectory(proceduresDirectory);
            Directory.CreateDirectory(functionsDirectory);

            // Create snapshot output files for Tables
            snapshots.Where(x => x.Type == SnapshotType.Table).ToList()
                .ForEach(x => WriteSnapshotFile(x, tablesDirectory));

            // Create snapshot output files for Views
            snapshots.Where(x => x.Type == SnapshotType.View).ToList()
                .ForEach(x => WriteSnapshotFile(x, viewsDirectory));

            // Create snapshot output files for Procedures
            snapshots.Where(x => x.Type == SnapshotType.Procedure).ToList()
                .ForEach(x => WriteSnapshotFile(x, proceduresDirectory));

            // Create snapshot output files for Functions
            snapshots.Where(x => x.Type == SnapshotType.Function).ToList()
                .ForEach(x => WriteSnapshotFile(x, functionsDirectory));
        }

        private static void WriteSnapshotFile(Snapshot snapshot, string outputPath)
        {
            var fileName = $"{snapshot.Type.ToString().Substring(0, 1)}_{snapshot.Order}__{snapshot.Description.Replace(" ", "_")}.sql";
            var filePath = Path.Combine(outputPath, fileName);
            File.WriteAllText(filePath, snapshot.ContentDDL);
        }

        private static Snapshot ParseSnapshot(string filePath)
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
