using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class MsSQLDbEngine(SchemaVersionerContext context) : BaseDbEngine(context), IDbEngine
    {
    }
}
