using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseCommand(IConnector connector, IDbEngine dbEngine, ILogger logger)
    {
        protected readonly IConnector _connector = connector;
        protected readonly IDbEngine _dbEngine = dbEngine;
        protected readonly ILogger _logger = logger;
    }
}
