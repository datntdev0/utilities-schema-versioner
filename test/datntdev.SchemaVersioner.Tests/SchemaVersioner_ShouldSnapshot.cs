using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldSnapshot : SQLiteConnectionFixture
    {
        [Fact]
        public void ShouldSnapshot_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            var output = GetSchemaVersioner(settings).Snapshot();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
