using datntdev.SchemaVersioner.Interfaces;
using Microsoft.Extensions.Logging;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class MsSQLDbEngine(BaseConnector connector, ILogger logger)
        : BaseDbEngine(connector, logger), IDbEngine
    {
    }
}
