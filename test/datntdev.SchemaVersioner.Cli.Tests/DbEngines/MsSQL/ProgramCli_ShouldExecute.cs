using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsSQL
{
    public class ProgramCli_ShouldExecute(DbContainer container) 
        : DockerConnectionFixture<DbContainer>(container), IClassFixture<DbContainer>
    {
        private string[] _defaultArgs => [
            $"--database-type=mssql",
            $"--metadata-schema=log",
            $"--metadata-table=MigrationHistory",
            $"--connection-string={_container.ConnectionString}",
            $"--migration-paths=Resources/MsSQL/Migrations;Resources/MsSQL/Repeatable",
            $"--snapshot-paths=Resources/MsSQL/Snapshots",
        ];

        [Fact]
        public void _01_ShouldInit_Successfully()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "init"];

            // Act
            Program.Main(args);

            // Assert
            AssertLatestSchema();
        }

        [Fact]
        public void _02_ShouldInit_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "init"];

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }

        [Fact]  
        public void _03_ShouldErase_Successfully()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "erase"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetTablesAndViews();
            Assert.Empty(collection);

            collection = GetFunctionsAndProcedures();
            Assert.Empty(collection);
        }

        [Fact]
        public void _04_ShouldRepair_Successfully()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "repair"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0"
                && x.description == "First migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "sa");
        }

        [Fact]
        public void _05_ShouldValidate_AllMigration_AreSuccess()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "validate"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0"
                && x.description == "First migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "sa");
        }

        [Fact]
        public void _06_ShouldValidate_OneMigration_ChecksumMismatch()
        {
            // Arrange
            ExecuteQuery("UPDATE [log].[MigrationHistory] SET checksum = 'invalidchecksum' WHERE version = '1.0.0';");

            string[] args = [.. _defaultArgs, "validate"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0" && x.checksum == "invalidchecksum");
        }

        [Fact]
        public void _07_ShouldValidate_AllMigration_ArePending()
        {
            // Arrange
            ExecuteNonQuery("DELETE FROM log.MigrationHistory;");
            
            string[] args = [.. _defaultArgs, "validate"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetMetadata();
            Assert.Empty(collection);
        }

        [Fact]
        public void _08_ShouldUpgrade_Successfully_UpgradeToTargetVersion()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "mssql",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _container.ConnectionString,
                "--migration-paths", "Resources/MsSQL/Migrations",
                "--snapshot-paths", "Resources/MsSQL/SnapshotsOutput",
                "--target-version=1.0.0",
                "upgrade"
            };

            // Act
            Program.Main(args);

            // Assert
            var collection = GetTablesAndViews();
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.DoesNotContain(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.DoesNotContain(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "dbo");
            Assert.DoesNotContain(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.DoesNotContain(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "dbo");

            collection = GetFunctionsAndProcedures();
            Assert.DoesNotContain(collection, x => x.name == "Procedure1" && x.type == "PROCEDURE" && x.schema == "dbo");
            Assert.DoesNotContain(collection, x => x.name == "CountTableRecords" && x.type == "FUNCTION" && x.schema == "dbo");

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0"
                && x.description == "First migration"
                && x.installed_by == "sa");
            Assert.DoesNotContain(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "sa");
            Assert.DoesNotContain(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "sa");
        }

        [Fact]
        public void _09_ShouldUpgrade_Successfully_UpgradeToLatestVersion()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--snapshots-as-repeatable", "Function;Procedure", "upgrade"];

            // Act
            Program.Main(args);

            // Assert
            AssertLatestSchema();
            var collection = GetMetadata();
            Assert.Contains(collection, x => x.description == "F_001__Function.sql"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.description == "P_001__Procedure.sql"
                && x.installed_by == "sa");
        }

        [Fact]
        public void _10_ShouldUpgrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--target-version=2.0.0", "upgrade"];
            

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration scripts.", ex.Message);
        }

        [Fact]
        public void _11_ShouldSnapshot_Successfully()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--snapshot-output-path=Resources/MsSQL/SnapshotsOutput", "snapshot"];

            // Act
            Program.Main(args);

            // Assert
            var collection = Directory.GetFiles("Resources/MsSQL/SnapshotsOutput", "*.sql", SearchOption.AllDirectories);
            Assert.Equal(8, collection.Length);
            Assert.Contains(collection, file => file.Contains("T_001__Table1.sql"));
            Assert.Contains(collection, file => file.Contains("T_002__Table2.sql"));
            Assert.Contains(collection, file => file.Contains("V_001__View1.sql"));
            Assert.Contains(collection, file => file.Contains("V_002__View1_1.sql"));
            Assert.Contains(collection, file => file.Contains("V_003__View2.sql"));
            Assert.Contains(collection, file => file.Contains("V_004__View2_1.sql"));
            Assert.Contains(collection, file => file.Contains("P_001__Procedure1.sql"));
            Assert.Contains(collection, file => file.Contains("F_001__CountTableRecords.sql"));
        }

        [Fact]
        public void _12_ShouldSnapshot_Successfully_RunInitFromSnapshots()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "mssql",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _container.ConnectionString,
                "--migration-paths", "Resources/MsSQL/Migrations",
                "--snapshot-paths", "Resources/MsSQL/SnapshotsOutput",
            };
            Program.Main([.. args, "erase"]);

            // Act
            Program.Main([.. args, "init"]);

            // Assert
            AssertLatestSchema();
        }

        [Fact]
        public void _13_ShouldDowngrade_Successfully_DowngradeTheLatestVersion() 
        {
            // Arrange

            string[] args = [.. _defaultArgs, "downgrade"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetTablesAndViews();
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "dbo");

            collection = GetFunctionsAndProcedures();
            Assert.Empty(collection);

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0"
                && x.description == "First migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "sa");
            Assert.DoesNotContain(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "sa");
        }

        [Fact]
        public void _14_ShouldDowngrade_Successfully_DowngradeToTargetVersion()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--target-version=1.0.0", "downgrade"];
         
            // Act
            Program.Main(args);

            // Assert
            var collection = GetTablesAndViews();
            Assert.Single(collection);

            collection = GetFunctionsAndProcedures();
            Assert.Empty(collection);

            collection = GetMetadata();
            Assert.Empty(collection);
        }

        [Fact]
        public void _15_ShouldDowngrade_RisedException_WhenTargetVersionNotExists()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--target-version=0.0.1", "downgrade"];

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '0.0.1' does not exist in migration history.", ex.Message);
        }

        protected override IEnumerable<dynamic> GetFunctionsAndProcedures()
        {
            return ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable()
                .Select(x => new 
                {
                    name = x["ROUTINE_NAME"].ToString(),  
                    type = x["ROUTINE_TYPE"].ToString(),
                    schema = x["ROUTINE_SCHEMA"].ToString()
                });
        }

        protected override IEnumerable<dynamic> GetMetadata()
        {
            return ExecuteQuery("SELECT * FROM log.MigrationHistory;").AsEnumerable()
                .Select(x => new 
                {
                    version = x["version"].ToString(),  
                    description = x["description"].ToString(),
                    installed_by = x["installed_by"].ToString(),
                    checksum = x["checksum"]?.ToString() ?? string.Empty
                });
        }

        protected override IEnumerable<dynamic> GetTablesAndViews()
        {
            return ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable()
                .Select(x => new 
                {
                    name = x["TABLE_NAME"].ToString(),  
                    type = x["TABLE_TYPE"].ToString(),
                    schema = x["TABLE_SCHEMA"].ToString()
                });
        }

        private void AssertLatestSchema()
        {
            var collection = GetTablesAndViews();
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "dbo");

            collection = GetFunctionsAndProcedures();
            Assert.Contains(collection, x => x.name == "Procedure1" && x.type == "PROCEDURE" && x.schema == "dbo");
            Assert.Contains(collection, x => x.name == "CountTableRecords" && x.type == "FUNCTION" && x.schema == "dbo");

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0"
                && x.description == "First migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "sa");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "sa");
        }
    }
}
