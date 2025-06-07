using datntdev.SchemaVersioner.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;

namespace datntdev.SchemaVersioner.Models
{
    internal class SchemaVersionerContext
    {
        public ILogger Logger { get; set; } = default!;
        public IDbConnection DbConnection { get; set; } = default!;
        public IConnector Connector { get; set; } = default!;
        public IDbEngine DbEngine { get; set; } = default!;
        public Settings Settings { get; set; } = default!;
    }
}
