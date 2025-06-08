using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldValidate : SQLiteConnectionFixture
    {
        [Fact]
        public void ShouldValidate_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            var output = GetSchemaVersioner(settings).Validate();

            // Assert
            Assert.NotNull(output.Data);
        }
    }
}
