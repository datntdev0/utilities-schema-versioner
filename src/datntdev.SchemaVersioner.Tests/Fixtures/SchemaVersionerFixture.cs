using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace datntdev.SchemaVersioner.Tests.Fixtures
{
    public class SchemaVersionerFixture : IDisposable
    {
        protected readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        public SchemaVersioner GetSchemaVersioner(Settings settings)
        {
            return new SchemaVersioner(_loggerMock.Object, settings);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
