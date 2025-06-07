using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal class BaseDbEngine(SchemaVersionerContext context)
    {
        protected readonly IConnector _connector = context.Connector;
        protected readonly ILogger _logger = context.Logger;
        protected readonly Settings _settings = context.Settings;
        protected readonly BaseConnector _baseConnector = (BaseConnector)context.Connector;
    }
}
