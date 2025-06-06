using datntdev.SchemaVersioner.Tests.Fixtures;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldInit : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldInit_Successfully()
        {
            // Act
            GetSchemaVersioner().Init();
        }
    }
}
