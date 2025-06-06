using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldInfo : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldInfo_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            GetSchemaVersioner(settings).Info();
        }
    }
}
