using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal class BaseCommand(ILogger logger)
    {
        protected readonly ILogger _logger = logger;
    }
}
