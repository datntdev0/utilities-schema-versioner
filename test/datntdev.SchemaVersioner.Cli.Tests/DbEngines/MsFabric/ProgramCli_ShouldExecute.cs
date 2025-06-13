using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsFabric
{
    public class ProgramCli_ShouldExecute(DbConnection connection)
        : ActualConnectionFixture<DbConnection>(connection), IClassFixture<DbConnection>
    {
        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _01_ShouldInit_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "init",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table1" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table2" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "Procedure1" &&
                row["ROUTINE_TYPE"].ToString() == "PROCEDURE" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "CountTableRecords" &&
                row["ROUTINE_TYPE"].ToString() == "FUNCTION" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");

            dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
            var thirdMigration = dataTable.First(row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration");
            Assert.NotEmpty(thirdMigration.Field<string>("checksum")!);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _02_ShouldInit_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "init",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _03_ShouldErase_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "erase",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.DoesNotContain(dataTable, row => row["TABLE_NAME"].ToString() == "MigrationHistory");
            Assert.DoesNotContain(dataTable, row => row["TABLE_NAME"].ToString() == "Table1");
            Assert.DoesNotContain(dataTable, row => row["TABLE_NAME"].ToString() == "Table2");
            Assert.DoesNotContain(dataTable, row => row["TABLE_NAME"].ToString() == "View1");
            Assert.DoesNotContain(dataTable, row => row["TABLE_NAME"].ToString() == "View2");
            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.DoesNotContain(dataTable, row => row["ROUTINE_NAME"].ToString() == "Procedure1");
            Assert.DoesNotContain(dataTable, row => row["ROUTINE_NAME"].ToString() == "CountTableRecords");
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _04_ShouldRepair_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "repair",
            };

            // Act
            Program.Main(args);                         

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
            var thirdMigration = dataTable.First(row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration");
            Assert.NotEmpty(thirdMigration.Field<string>("checksum")!);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _05_ShouldValidate_AllMigration_AreSuccess()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _06_ShouldValidate_OneMigration_ChecksumMismatch()
        {
            // Arrange
            ExecuteQuery("UPDATE log.MigrationHistory SET checksum = 'invalidchecksum' WHERE version = '1.0.0';");

            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _07_ShouldValidate_AllMigration_ArePending()
        {
            // Arrange
            ExecuteNonQuery("DELETE FROM log.MigrationHistory;");
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _08_ShouldUpgrade_Successfully_UpgradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations",
                "--target-version", "1.0.0",
                "upgrade",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();

            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);

            Assert.DoesNotContain(dataTable, row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration");
            Assert.DoesNotContain(dataTable, row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table1" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.DoesNotContain(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table2" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.DoesNotContain(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.DoesNotContain(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "Procedure1" &&
                row["ROUTINE_TYPE"].ToString() == "PROCEDURE" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");
            Assert.DoesNotContain(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "CountTableRecords" &&
                row["ROUTINE_TYPE"].ToString() == "FUNCTION" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _09_ShouldUpgrade_Successfully_UpgradeToLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "upgrade",
            };
            // Act
            Program.Main(args);
            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
            var thirdMigration = dataTable.First(row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration");
            Assert.NotEmpty(thirdMigration.Field<string>("checksum")!);

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table1" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table2" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1_1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2_1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "Procedure1" &&
                row["ROUTINE_TYPE"].ToString() == "PROCEDURE" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "CountTableRecords" &&
                row["ROUTINE_TYPE"].ToString() == "FUNCTION" &&
                row["ROUTINE_SCHEMA"].ToString() == "dbo");
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _10_ShouldUpgrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--target-version", "2.0.0",
                "upgrade",
            };
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration scripts.", ex.Message);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _11_ShouldDowngrade_Successfully_DowngradeTheLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations",
                "downgrade",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);

            Assert.DoesNotContain(dataTable, row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table1" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table2" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1_1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2_1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.Empty(dataTable);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _12_ShouldDowngrade_Successfully_DowngradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--target-version", "1.0.0",
                "downgrade",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable();
            Assert.Empty(dataTable);

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Single(dataTable, row =>
                row["TABLE_SCHEMA"].ToString() == "log" &&
                row["TABLE_NAME"].ToString() == "MigrationHistory");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.Empty(dataTable);
        }

        [Fact(Skip = "Should setup Fabric Connection from Cloud")]
        public void _13_ShouldDowngrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--connection-string", _dbConnection.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--target-version", "2.0.0",
                "downgrade",
            };

            // Act & Assert

            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration history.", ex.Message);
        }
    }
}
