using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldUpgrade : SQLiteConnectionFixture
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
