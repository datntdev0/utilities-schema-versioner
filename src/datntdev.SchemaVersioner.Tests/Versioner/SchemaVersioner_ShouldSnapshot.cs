using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldSnapshot : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldSnapshot_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            GetSchemaVersioner(settings).Snapshot();
        }
    }
}
