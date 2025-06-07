using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Cli
{
    internal class Program
    {
        private const string AvailableCommands = "Available commands: init, upgrade, downgrade, validate, repair, erase, snapshot.";

        static void Main(string[] args)
        {
            using var factory = LoggerFactory.Create(ConfigureConsoleLogger);
            var logger = factory.CreateLogger(nameof(SchemaVersioner));

            var versioner = SchemaVersionerFactory.GetSchemaVersioner(args, logger);

            switch (args.LastOrDefault()?.ToLower())
            {
                case "init":
                    versioner.Init();
                    break;
                case "upgrade":
                    versioner.Upgrade();
                    break;
                case "downgrade":
                    versioner.Downgrade();
                    break;
                case "validate":
                    versioner.Validate();
                    break;
                case "repair":
                    versioner.Repair();
                    break;
                case "erase":
                    versioner.Erase();
                    break;
                case "snapshot":
                    versioner.Snapshot();
                    break;
                case null:
                    logger.LogError($"No command provided. {AvailableCommands}");
                    break;
                default:
                    logger.LogError($"Unknown command. {AvailableCommands}");
                    break;
            }
        }

        private static void ConfigureConsoleLogger(ILoggingBuilder builder)
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.IncludeScopes = true;
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            });
        }
    }
}
