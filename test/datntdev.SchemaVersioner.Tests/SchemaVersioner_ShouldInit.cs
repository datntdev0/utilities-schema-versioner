using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldInit : SQLiteConnectionFixture
    {
        [Fact]
        public void ShouldInit_1_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
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
        public void ShouldInit_2_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Loaders/Snapshots"],
                MigrationPaths = ["Loaders/Migrations"],
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => GetSchemaVersioner(settings).Init());
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }
    }
}
