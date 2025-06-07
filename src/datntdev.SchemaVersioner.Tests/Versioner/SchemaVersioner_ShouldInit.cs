using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldInit : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldInit_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
            };

            // Act
            var output = GetSchemaVersioner(settings).Init();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
