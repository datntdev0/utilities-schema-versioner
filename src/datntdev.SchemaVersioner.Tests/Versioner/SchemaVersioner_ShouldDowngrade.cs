using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldDowngrade : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldDowngrade_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
            };

            // Act
            var output = GetSchemaVersioner(settings).Downgrade();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
