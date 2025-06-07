using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseCommand(IConnector connector, ILogger logger)
    {
        protected readonly ILogger _logger = logger;
        protected readonly IConnector _connector = connector;
    }
}
