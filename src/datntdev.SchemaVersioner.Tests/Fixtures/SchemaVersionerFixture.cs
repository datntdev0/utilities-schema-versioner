using Microsoft.Extensions.Logging;
using Moq;

namespace datntdev.SchemaVersioner.Tests.Fixtures
{
    public class SchemaVersionerFixture : IDisposable
    {
        protected readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        public SchemaVersioner GetSchemaVersioner()
        {
            return new SchemaVersioner(_loggerMock.Object);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
