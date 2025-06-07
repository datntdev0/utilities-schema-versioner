using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldErase : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldErase_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
            };

            // Act
            var output = GetSchemaVersioner(settings).Erase();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
