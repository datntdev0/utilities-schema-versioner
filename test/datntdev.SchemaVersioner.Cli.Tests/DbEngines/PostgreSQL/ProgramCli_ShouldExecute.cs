using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.PostgreSQL
{
    public class ProgramCli_ShouldExecute(DbContainer container)
        : DockerConnectionFixture<DbContainer>(container), IClassFixture<DbContainer>
    {
        private string[] _defaultArgs => [
            $"--database-type=postgresql",
            $"--metadata-schema=log",
            $"--metadata-table=MigrationHistory",
            $"--connection-string={_container.ConnectionString}",
            $"--migration-paths=Resources/PostgeSQL/Migrations;Resources/PostgeSQL/Repeatable",
            $"--snapshot-paths=Resources/PostgeSQL/Snapshots",
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
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "postgres");
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
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration"
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration"
                && x.installed_by == "postgres");
        }

        [Fact]
        public void _06_ShouldValidate_OneMigration_ChecksumMismatch()
        {
            // Arrange
            ExecuteNonQuery(@"UPDATE ""log"".""MigrationHistory"" SET checksum = 'invalid-checksum' WHERE version = '1.0.0';");

            string[] args = [.. _defaultArgs, "validate"];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0" && x.checksum == "invalid-checksum");
        }

        [Fact]
        public void _07_ShouldValidate_AllMigration_ArePending()
        {
            // Arrange
            ExecuteNonQuery(@"DELETE FROM ""log"".""MigrationHistory"";");

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
            string[] args = [
                $"--database-type=postgresql",
                $"--metadata-schema=log",
                $"--metadata-table=MigrationHistory",
                $"--connection-string={_container.ConnectionString}",
                $"--migration-paths=Resources/PostgeSQL/Migrations",
                $"--target-version=1.0.0",
                $"upgrade",
            ];

            // Act
            Program.Main(args);

            // Assert
            var collection = GetTablesAndViews();
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "public");
            Assert.DoesNotContain(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.DoesNotContain(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "public");
            Assert.DoesNotContain(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "public");
            Assert.DoesNotContain(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "public");

            collection = GetFunctionsAndProcedures();
            Assert.DoesNotContain(collection, x => x.name == "Procedure1" && x.type == "PROCEDURE" && x.schema == "public");
            Assert.DoesNotContain(collection, x => x.name == "CountTableRecords" && x.type == "FUNCTION" && x.schema == "public");

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0" 
                && x.description == "First migration" 
                && x.installed_by == "postgres");
            Assert.DoesNotContain(collection, x => x.version == "1.1.0"
                && x.description == "Second migration" 
                && x.installed_by == "postgres");
            Assert.DoesNotContain(collection, x => x.version == "1.2.0"
                && x.description == "Third migration" 
                && x.installed_by == "postgres");
        }

        [Fact]
        public void _09_ShouldUpgrade_Successfully_UpgradeToLatestVersion()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "upgrade"];

            // Act
            Program.Main(args);

            // Assert
            AssertLatestSchema();
        }

        [Fact]
        public void _10_ShouldUpgrade_RisedException_WhenTargetVersionNotFound()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--target-version=2.0.0", "upgrade"];
            
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '2.0.0' does not exist in migration scripts.", ex.Message);
        }

        [Fact]
        public void _11_ShouldSnapshot_RisedException_NotSupportedException()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "snapshot"];

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => Program.Main(args));
            Assert.Equal("Snapshot feature is not supported for PostgreSQL.", ex.Message);
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
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "public");

            collection = GetFunctionsAndProcedures();
            Assert.Empty(collection);

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0" 
                && x.description == "First migration" 
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.1.0"
                && x.description == "Second migration" 
                && x.installed_by == "postgres");
            Assert.DoesNotContain(collection, x => x.version == "1.2.0"
                && x.description == "Third migration" 
                && x.installed_by == "postgres");
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
        public void _15_ShouldDowngrade_RisedException_WhenTargetVersionNotFound()
        {
            // Arrange
            string[] args = [.. _defaultArgs, "--target-version=0.0.1", "downgrade"];
           
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Target version '0.0.1' does not exist in migration history.", ex.Message);
        }

        protected override IEnumerable<dynamic> GetFunctionsAndProcedures()
        {
            var sql = @"
                SELECT ROUTINE_NAME, ROUTINE_TYPE, ROUTINE_SCHEMA
                FROM INFORMATION_SCHEMA.ROUTINES
                WHERE ROUTINE_SCHEMA NOT IN ('pg_catalog', 'information_schema')
                ORDER BY ROUTINE_NAME;";
            return ExecuteQuery(sql).AsEnumerable()
                .Select(row => new
                {
                    name = row["ROUTINE_NAME"].ToString(),
                    type = row["ROUTINE_TYPE"].ToString(),
                    schema = row["ROUTINE_SCHEMA"].ToString()
                });
        }

        protected override IEnumerable<dynamic> GetMetadata()
        {
            return ExecuteQuery(@"SELECT * FROM ""log"".""MigrationHistory""").AsEnumerable()
                .Select(row => new
                {
                    version = row["version"].ToString(),
                    description = row["description"].ToString(),
                    installed_by = row["installed_by"].ToString(),
                    checksum = row["checksum"]?.ToString() ?? string.Empty
                });
        }

        protected override IEnumerable<dynamic> GetTablesAndViews()
        {
            var sql = @"
                SELECT TABLE_NAME, TABLE_TYPE, TABLE_SCHEMA
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA NOT IN ('pg_catalog', 'information_schema')
                ORDER BY TABLE_NAME;";
            return ExecuteQuery(sql).AsEnumerable()
                .Select(row => new
                {
                    name = row["TABLE_NAME"].ToString(),
                    type = row["TABLE_TYPE"].ToString(),
                    schema = row["TABLE_SCHEMA"].ToString()
                });
        }

        private void AssertLatestSchema()
        {
            var collection = GetTablesAndViews();
            Assert.Contains(collection, x => x.name == "Table1" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "Table2" && x.type == "BASE TABLE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View1" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View2" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View1_1" && x.type == "VIEW" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "View2_1" && x.type == "VIEW" && x.schema == "public");

            collection = GetFunctionsAndProcedures();
            Assert.Contains(collection, x => x.name == "Procedure1" && x.type == "PROCEDURE" && x.schema == "public");
            Assert.Contains(collection, x => x.name == "CountTableRecords" && x.type == "FUNCTION" && x.schema == "public");

            collection = GetMetadata();
            Assert.Contains(collection, x => x.version == "1.0.0" 
                && x.description == "First migration" 
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.1.0" 
                && x.description == "Second migration" 
                && x.installed_by == "postgres");
            Assert.Contains(collection, x => x.version == "1.2.0"
                && x.description == "Third migration" 
                && x.installed_by == "postgres");
        }
    }
}
