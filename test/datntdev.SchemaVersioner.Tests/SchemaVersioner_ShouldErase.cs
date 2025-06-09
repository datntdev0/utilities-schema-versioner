using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldErase : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=sqlite_erase;Mode=Memory;Cache=Shared";

        [Fact]
        public void ShouldErase_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            GetSchemaVersioner(settings).Init();

            // Act
            var output = GetSchemaVersioner(settings).Erase();

            // Assert
            Assert.NotNull(output.Data);

            var dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "schema_versioner_migrations");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View1");
            Assert.DoesNotContain(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View2");
        }
    }
}
