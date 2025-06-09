using datntdev.SchemaVersioner.Tests.Framework;
using System.Data;
using Xunit.Abstractions;

namespace datntdev.SchemaVersioner.Cli.Tests
{
    public class ProgramCli_ShouldExecute(ITestOutputHelper output) : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=Resources/SQLite/database.db;Cache=Shared";

        [Fact]
        public void _01_ShouldInit_Successfully()
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
        public void _02_ShouldInit_RisedException_WhenMetadataTableExists()
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
        public void _03_ShouldErase_Successfully()
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
        public void _04_ShouldRepair_Successfully()
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
            var dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
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
        public void _05_ShouldValidate_AllMigrations_AreSuccess()
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
        public void _06_ShouldValidate_OneMigration_ChecksumMismatch()
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
        public void _07_ShouldValidate_AllMigrations_ArePending()
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

        [Fact]
        public void _08_ShouldUpgrade_Successfully_UpgradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--target-version", "1.0.0",
                "upgrade",
            };
            // Act
            Program.Main(args);
            // Assert
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
        }

        [Fact]
        public void _09_ShouldUpgrade_Successfully_UpgradeToLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "upgrade",
            };
            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM schema_versioner_migrations").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.0.0"
                && row.Field<string>("description") == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row.Field<string>("version") == "1.1.0"
                && row.Field<string>("description") == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);

            dataTable = ExecuteQuery("SELECT * FROM sqlite_master").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table1");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View1");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "table"
                && row.Field<string>("tbl_name") == "Table2");
            Assert.Contains(dataTable, row =>
                row.Field<string>("type") == "view"
                && row.Field<string>("tbl_name") == "View2");
        }

        [Fact]
        public void _10_ShouldUpgrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--target-version", "2.0.0",
                "upgrade",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration scripts.", ex.Message);
        }

        [Fact]
        public void _11_ShouldDowngrade_Successfully_DowngradeTheLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "downgrade",
            };

            // Act
            Program.Main(args);

            // Assert
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
        public void _12_ShouldDowngrade_Successfully_DowngradeTheTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--target-version", "1.0.0",
                "downgrade",
            };
            // Act
            Program.Main(args);

            // Assert
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
        public void _13_ShouldDowngrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--target-version", "2.0.0", // This version does not exist
                "downgrade",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration history.", ex.Message);
        }

        [Fact]
        public void _14_ShouldSnapshot_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "sqlite",
                "--connection-string", SQLiteConnectionString,
                "--migration-paths", "Resources/SQLite/Migrations",
                "--snapshot-paths", "Resources/SQLite/Snapshots",
                "--snapshot-output-path", "Resources/SQLite/SnapshotsOutput",
            };
            Program.Main([.. args, "erase"]);
            Program.Main([.. args, "init"]);

            // Act
            Program.Main([.. args, "snapshot"]);

            // Assert
            var snapshotOutputFiles = Directory.GetFiles(
                "Resources/SQLite/SnapshotsOutput", "*.sql", SearchOption.AllDirectories);
            Assert.Equal(4, snapshotOutputFiles.Length);
            Assert.Contains(snapshotOutputFiles, file => file.Contains("T__001_Table1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("T__002_Table2.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V__001_View1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V__002_View2.sql"));
        }
    }
}
