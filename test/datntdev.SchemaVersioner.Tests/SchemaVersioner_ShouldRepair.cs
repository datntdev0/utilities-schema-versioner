using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldRepair : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=sqlite_repair;Mode=Memory;Cache=Shared";

        [Fact]
        public void ShouldRepair_1_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };

            // Act
            var output = GetSchemaVersioner(settings).Repair();

            // Assert
            Assert.NotNull(output.Data);

            var dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "schema_versioner_migrations");

            dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.0.0"
                && row.Field<string>("description") == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.1.0"
                && row.Field<string>("description") == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
        }

        [Fact]
        public void ShouldRepair_2_Successfully_WhenMetadataIsCorrupted()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            ExecuteNonQuery("DELETE FROM schema_versioner_migrations WHERE version = '1.0.0'");

            // Act
            var output = GetSchemaVersioner(settings).Repair();

            // Assert
            Assert.NotNull(output.Data);

            var dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "schema_versioner_migrations");

            dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.0.0"
                && row.Field<string>("description") == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.1.0"
                && row.Field<string>("description") == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
        }
    }
}
