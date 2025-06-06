using datntdev.SchemaVersioner.Models;
using System.IO;
using System.Linq;

namespace datntdev.SchemaVersioner.Loaders
{
    internal class MigrationLoader
    {
        public Migration[] Load(Models.Configuration.Settings settings) 
        {
            return settings.MigrationPaths
                .SelectMany(x => Directory.GetFiles(x, "*.sql", SearchOption.AllDirectories))
                .Select(x => ParseMigration(Path.GetFullPath(x)))
                .ToArray();
        }

        private Migration ParseMigration(string filePath)
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
            var description = splited[1].Replace("_", " ");
            var version = type == MigrationType.Repeatable ? string.Empty :
                splited[0].Replace(prefix, string.Empty).Replace("_", ".")[1..];

            return new Migration()
            {
                Type = type,
                Version = version ,
                Description = description,
                FilePath = filePath,
            };
        }
    }
}
