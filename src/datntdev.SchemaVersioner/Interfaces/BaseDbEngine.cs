using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal class BaseDbEngine(BaseConnector connector, ILogger logger)
    {
        protected readonly BaseConnector _connector = connector;
        protected readonly ILogger _logger = logger;
    }
}
