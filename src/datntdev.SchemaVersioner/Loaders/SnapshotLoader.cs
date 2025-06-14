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
            Directory.Delete(settings.SnapshotOutputPath, true);
            Directory.CreateDirectory(settings.SnapshotOutputPath);

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

            // Create snapshot output files for Procedures
            var proceduresDirectory = $"{settings.SnapshotOutputPath}/Procedures";
            if (!Directory.Exists(proceduresDirectory)) Directory.CreateDirectory(proceduresDirectory);
            snapshots.Where(x => x.Type == SnapshotType.Procedure)
                .ToList()
                .ForEach(x => WriteSnapshotFile(x, proceduresDirectory));

            // Create snapshot output files for Functions
            var functionsDirectory = $"{settings.SnapshotOutputPath}/Functions";
            if (!Directory.Exists(functionsDirectory)) Directory.CreateDirectory(functionsDirectory);
            snapshots.Where(x => x.Type == SnapshotType.Function)
                .ToList()
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
