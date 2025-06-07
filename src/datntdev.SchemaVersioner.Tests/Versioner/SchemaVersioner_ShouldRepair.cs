using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldRepair : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldRepair_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
            };

            // Act
            var output = GetSchemaVersioner(settings).Repair();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
