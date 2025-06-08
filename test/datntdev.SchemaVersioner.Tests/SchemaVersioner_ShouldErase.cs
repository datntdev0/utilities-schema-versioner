using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldErase : SQLiteConnectionFixture
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
