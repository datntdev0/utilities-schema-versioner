using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.PostgreSQL
{
    public class ProgramCli_ShouldExecute(DbContainer container) 
        : DockerConnectionFixture<DbContainer>(container), IClassFixture<DbContainer>
    {
        [Fact]
        public void _01_ShouldInit_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "postgresql",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _container.ConnectionString,
                "--migration-paths", "Resources/PostgeSQL/Migrations;Resources/PostgeSQL/Repeatable",
                "--snapshot-paths", "Resources/PostgeSQL/Snapshots",
                "init",
            };

            // Act
            Program.Main(args);

            // Assert
            var dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.TABLES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table1" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "public");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "Table2" &&
                row["TABLE_TYPE"].ToString() == "BASE TABLE" &&
                row["TABLE_SCHEMA"].ToString() == "public");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View1" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "public");
            Assert.Contains(dataTable, row =>
                row["TABLE_NAME"].ToString() == "View2" &&
                row["TABLE_TYPE"].ToString() == "VIEW" &&
                row["TABLE_SCHEMA"].ToString() == "public");

            dataTable = ExecuteQuery("SELECT * FROM INFORMATION_SCHEMA.ROUTINES;").AsEnumerable();
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "Procedure1" &&
                row["ROUTINE_TYPE"].ToString() == "PROCEDURE" &&
                row["ROUTINE_SCHEMA"].ToString() == "public");
            Assert.Contains(dataTable, row =>
                row["ROUTINE_NAME"].ToString() == "CountTableRecords" &&
                row["ROUTINE_TYPE"].ToString() == "FUNCTION" &&
                row["ROUTINE_SCHEMA"].ToString() == "public");

            dataTable = ExecuteQuery(@"SELECT * FROM log.""MigrationHistory"";").AsEnumerable();
            var firstMigration = dataTable.First(row =>
                row["version"].ToString() == "1.0.0" &&
                row["description"].ToString() == "First migration" &&
                row["installed_by"].ToString() == "postgres");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration" &&
                row["installed_by"].ToString() == "postgres");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
            var thirdMigration = dataTable.First(row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration" &&
                row["installed_by"].ToString() == "postgres");
            Assert.NotEmpty(thirdMigration.Field<string>("checksum")!);
        }

        [Fact]
        public void _02_ShouldInit_RisedException_WhenMetadataTableExists()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "postgresql",
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--connection-string", _container.ConnectionString,
                "--migration-paths", "Resources/PostgeSQL/Migrations;Resources/PostgeSQL/Repeatable",
                "--snapshot-paths", "Resources/PostgeSQL/Snapshots",
                "init",
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => Program.Main(args));
            Assert.Equal("Metadata table already exists. We only initialize for new database", ex.Message);
        }

        protected override IEnumerable<dynamic> GetFunctionsAndProcedures()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<dynamic> GetMetadata()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<dynamic> GetTablesAndViews()
        {
            throw new NotImplementedException();
        }
    }
}
