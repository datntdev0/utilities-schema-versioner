using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var factory = LoggerFactory.Create(ConfigureConsoleLogger);
            var logger = factory.CreateLogger("SchemaVersioner.Cli");

            var versioner = new SchemaVersioner(logger);
            if (args.Length == 0)
            {
                logger.LogError("No command provided. Available commands: init, upgrade, downgrade, validate, repair, snapshot.");
                return;
            }

            using (logger.BeginScope(nameof(SchemaVersioner)))
            {
                switch (args[0].ToLower())
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
                    case "snapshot":
                        versioner.Snapshot();
                        break;
                    default:
                        logger.LogError("Unknown command. Available commands: init, upgrade, downgrade, validate, repair, snapshot.");
                        break;
                }
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
