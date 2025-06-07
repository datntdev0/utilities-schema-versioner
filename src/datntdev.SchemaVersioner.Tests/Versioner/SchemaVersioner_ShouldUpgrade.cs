using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldUpgrade : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldUpgrade_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            var output = GetSchemaVersioner(settings).Upgrade();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
