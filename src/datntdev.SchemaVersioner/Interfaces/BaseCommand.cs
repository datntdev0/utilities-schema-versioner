using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseCommand(SchemaVersionerContext context)
    {
        protected readonly IConnector _connector = context.Connector;
        protected readonly IDbEngine _dbEngine = context.DbEngine;
        protected readonly ILogger _logger = context.Logger;
        protected readonly Settings _settings = context.Settings;
    }
}
