using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;
using Xunit.Abstractions;

namespace datntdev.SchemaVersioner.Cli.Tests
{
    public class ProgramCli_ShouldExecute(ITestOutputHelper output) : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=Resources/SQLite/database.db;Cache=Shared";

        [Fact]
        public void _1_ShouldInit_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "init",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "schema_versioner_migrations");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");

            dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
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
        public void _2_ShouldInit_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "init",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }

        [Fact]
        public void _3_ShouldErase_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "erase",
            };

            // Act
            Program.Main(args);

            // Assert
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

        [Fact]
        public void _4_ShouldRepair_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "repair",
            };

            // Act
            Program.Main(args);

            // Assert
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
        public void _5_ShouldValidate_AllMigrations_AreSuccess()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact]
        public void _6_ShouldValidate_OneMigration_ChecksumMismatch()
        {
            // Arrange
            ExecuteNonQuery("UPDATE schema_versioner_migrations SET checksum = 'invalid_checksum' WHERE version = '1.0.0'");
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact]
        public void _7_ShouldValidate_AllMigrations_ArePending()
        {
            // Arrange
            ExecuteNonQuery("DELETE FROM schema_versioner_migrations");
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }
    }
}
