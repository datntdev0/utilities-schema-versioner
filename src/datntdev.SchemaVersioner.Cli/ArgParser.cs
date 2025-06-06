using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Cli
{
    internal static class ArgParser
    {
        public static Settings FromArgs(string[] args)
        {
            var settings = new Settings();
            var dict = new Dictionary<string, Action<string>>
            {
                ["--migration-table"] = val => settings.MigrationTable = val,
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
    }
}
