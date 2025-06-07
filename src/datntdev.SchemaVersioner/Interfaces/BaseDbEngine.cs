using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal class BaseDbEngine(SchemaVersionerContext context)
    {
        protected readonly IConnector _connector = context.Connector;
        protected readonly IConnector _logger = context.Connector;
    }
}
