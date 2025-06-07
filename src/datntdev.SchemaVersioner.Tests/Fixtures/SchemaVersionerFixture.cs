using datntdev.SchemaVersioner.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace datntdev.SchemaVersioner.Tests.Fixtures
{
    public class SchemaVersionerFixture : IDisposable
    {
        protected readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        protected readonly IDbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

        public SchemaVersioner GetSchemaVersioner(Settings settings)
        {
            return new SchemaVersioner(_dbConnection, _loggerMock.Object, settings);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
