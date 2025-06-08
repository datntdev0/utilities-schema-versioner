using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldRepair : SQLiteConnectionFixture
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
