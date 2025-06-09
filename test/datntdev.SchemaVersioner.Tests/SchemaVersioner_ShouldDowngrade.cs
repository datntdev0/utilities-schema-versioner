using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldDowngrade : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=sqlite_downgrade;Mode=Memory;Cache=Shared";

        [Fact]
        public void ShouldDowngrade_1_Successfully_DowngradeTheLatestVersion()
        {
            // Arrange
            var settings = new Settings()
            {
                MigrationPaths = ["Resources/SQLite/Migrations"],
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
            };
            // Ensure the database is initialized and has migrations applied
            GetSchemaVersioner(settings).Init();

            var dataTable1 = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();


            // Act
            var output = GetSchemaVersioner(settings).Downgrade();

            // Assert
            Assert.NotNull(output.Data);

            var dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();

            var firstMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.0.0"
                && row.Field<string>("description") == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);

            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("version") == "1.1.0"
                && row.Field<string>("description") == "Second migration");

            dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View1");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View2");
        }

        [Fact]
        public void ShouldDowngrade_2_Successfully_DowngradeTheTargetVersion()
        {
            // Arrange
            var settings = new Settings()
            {
                TargetVersion = "1.0.0",
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };

            // Act
            var output = GetSchemaVersioner(settings).Downgrade();
            
            // Assert
            Assert.NotNull(output.Data);
            
            var dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
            Assert.Empty(dataTable);

            dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View1");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View2");
        }

        [Fact]
        public void ShouldDowngrade_3_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var settings = new Settings()
            {
                TargetVersion = "2.0.0", // This version does not exist
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => GetSchemaVersioner(settings).Downgrade());
            Assert.Equal("Target version '2.0.0' does not exist in migration history.", ex.Message);
        }
    }
}
