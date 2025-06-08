using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldDowngrade : SQLiteConnectionFixture
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
