using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldValidate : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldValidate_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {

            };

            // Act
            GetSchemaVersioner(settings).Validate();
        }
    }
}
