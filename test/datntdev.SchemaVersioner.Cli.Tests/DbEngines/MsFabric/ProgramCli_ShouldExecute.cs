using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsFabric
{
    public class ProgramCli_ShouldExecute(DbConnection connection)
        : ActualConnectionFixture<DbConnection>(connection), IClassFixture<DbConnection>
    {
        // Add at the top of the file, inside the namespace but outside the class
        public const bool SkipTests = true;

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _01_ShouldInit_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
            };
            Program.Main([.. args, "erase"]);

            // Act
            Program.Main([.. args, "init"]);

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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _02_ShouldInit_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "init",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _03_ShouldErase_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _04_ShouldRepair_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _05_ShouldValidate_AllMigration_AreSuccess()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _06_ShouldValidate_OneMigration_ChecksumMismatch()
        {
            // Arrange
            ExecuteQuery("UPDATE log.MigrationHistory SET checksum = 'invalidchecksum' WHERE version = '1.0.0';");

            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _07_ShouldValidate_AllMigration_ArePending()
        {
            // Arrange
            ExecuteNonQuery("DELETE FROM log.MigrationHistory;");
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "validate",
            };

            // Act
            Program.Main(args);
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _08_ShouldUpgrade_Successfully_UpgradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _09_ShouldUpgrade_Successfully_UpgradeToLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _10_ShouldUpgrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations;Resources/MsFabric/Repeatable",
                "--target-version", "2.0.0",
                "upgrade",
            };
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration scripts.", ex.Message);
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _11_ShouldSnapshot_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations",
                "--snapshot-paths", "Resources/MsFabric/Snapshots",
                "--snapshot-output-path", "Resources/MsFabric/SnapshotsOutput",
                "snapshot",
            };

            // Act
            Program.Main(args);

            // Assert
            var snapshotOutputFiles = Directory.GetFiles(
                "Resources/MsFabric/SnapshotsOutput", "*.sql", SearchOption.AllDirectories);
            Assert.Equal(8, snapshotOutputFiles.Length);
            Assert.Contains(snapshotOutputFiles, file => file.Contains("T_001__Table1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("T_002__Table2.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V_001__View1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V_002__View1_1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V_003__View2.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("V_004__View2_1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("P_001__Procedure1.sql"));
            Assert.Contains(snapshotOutputFiles, file => file.Contains("F_001__CountTableRecords.sql"));
        }

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _12_ShouldSnapshot_Successfully_RunInitFromSnapshots()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
                "--migration-paths", "Resources/MsFabric/Migrations",
                "--snapshot-paths", "Resources/MsFabric/SnapshotsOutput",
            };
            Program.Main([.. args, "erase"]);

            // Act
            Program.Main([.. args, "init"]);

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
                row["TABLE_NAME"].ToString() == "View1_1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "dbo");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _13_ShouldDowngrade_Successfully_DowngradeTheLatestVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _14_ShouldDowngrade_Successfully_DowngradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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

        [Fact(Skip = SkipTests ? "Should setup Fabric Connection from Cloud" : null)]
        public void _15_ShouldDowngrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "msfabric",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _dbConnection.ConnectionString,
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
