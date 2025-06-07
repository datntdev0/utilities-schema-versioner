using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Fixtures;
using System.Data;

namespace datntdev.SchemaVersioner.Tests.Versioner
{
    public class SchemaVersioner_ShouldInit : SchemaVersionerFixture
    {
        [Fact]
        public void ShouldInit_Successfully()
        {
            _dbConnection.Close(); // Close connection to reset database
            
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Loaders/Snapshots"],
                MigrationPaths = ["Loaders/Migrations"],
            };

            // Act
            var output = GetSchemaVersioner(settings).Init();

            // Assert
            Assert.NotNull(output.Data);
            
            var dataTable = ExecuteReader("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "schema_versioner_migrations");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");

            dataTable = ExecuteReader("SELECT * FROM schema_versioner_migrations").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.0.0"
                && row.Field<string>("description") == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.1.0"
                && row.Field<string>("description") == "Second migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
        }

        [Fact]
        public void ShouldInit_RisedException_WhenMetadataTableExists()
        {
            _dbConnection.Close(); // Close connection to reset database
           
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Loaders/Snapshots"],
                MigrationPaths = ["Loaders/Migrations"],
            };
            GetSchemaVersioner(settings).Init();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => GetSchemaVersioner(settings).Init());
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }
    }
}
