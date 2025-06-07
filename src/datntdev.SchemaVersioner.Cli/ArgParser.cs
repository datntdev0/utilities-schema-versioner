using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Cli
{
    internal static class ArgParser
    {
        public static Settings GetSettings(string[] args)
        {
            var settings = new Settings();
            var dict = new Dictionary<string, Action<string>>
            {
                ["--migration-table"] = val => settings.MetadataTable = val,
                ["--migration-paths"] = val => settings.MigrationPaths = val.Split(';', ','),
                ["--snapshot-paths"] = val => settings.SnapshotPaths = val.Split(';', ',')
            };

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (dict.ContainsKey(arg) && i + 1 < args.Length)
                {
                    dict[arg](args[++i]);
                }
                else if (arg.StartsWith("--"))
                {
                    var split = arg.IndexOf('=') > 0 ? arg.Split('=', 2) : null;
                    if (split != null && dict.ContainsKey(split[0]))
                    {
                        dict[split[0]](split[1]);
                    }
                }
            }

            return settings;
        }

        public static SettingsCli GetSettingsCli(string[] args)
        {
            var settings = new SettingsCli();
            var dict = new Dictionary<string, Action<string>>
            {
                ["--database-type"] = val => settings.DbEngineType = ParseDbEngineType(val),
                ["--connection-string"] = val => settings.ConnectionString = val,
                ["--migration-table"] = val => settings.MetadataTable = val,
                ["--migration-table"] = val => settings.MetadataTable = val,
                ["--migration-paths"] = val => settings.MigrationPaths = val.Split(';', ','),
                ["--snapshot-paths"] = val => settings.SnapshotPaths = val.Split(';', ',')
            };

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (dict.ContainsKey(arg) && i + 1 < args.Length)
                {
                    dict[arg](args[++i]);
                }
                else if (arg.StartsWith("--"))
                {
                    var split = arg.IndexOf('=') > 0 ? arg.Split('=', 2) : null;
                    if (split != null && dict.ContainsKey(split[0]))
                    {
                        dict[split[0]](split[1]);
                    }
                }
            }

            return settings;
        }

        private static DbEngineType ParseDbEngineType(string value)
        {
            if (Enum.TryParse<DbEngineType>(value, ignoreCase: true, out var dbType))
            {
                return dbType;
            }
            else
            {
               return DbEngineType.None;
            }
        }
    }
}
