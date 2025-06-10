using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsSQL
{
    public class ProgramCli_ShouldExecute : DockerConnectionFixture<DbContainer>
    {
        [Fact]
        public void _01_ShouldInit_Successfully()
        {
            // Arrange
            var args = new string[]
            {
                "--database-type", "mssql",
                "--connection-string", _container.ConnectionString,
                "--metadata-schema", "log",
                "--metadata-table", "MigrationHistory",
                "--migration-paths", "Resources/MsSQL/Migrations;Resources/MsSQL/Repeatable",
                "--snapshot-paths", "Resources/MsSQL/Snapshots",
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
                row["description"].ToString() == "First migration" &&
                row["installed_by"].ToString() == "sa");
            Assert.NotEmpty(firstMigration.Field<string>("checksum")!);
            var secondMigration = dataTable.First(row =>
                row["version"].ToString() == "1.1.0" &&
                row["description"].ToString() == "Second migration" &&
                row["installed_by"].ToString() == "sa");
            Assert.NotEmpty(secondMigration.Field<string>("checksum")!);
            var thirdMigration = dataTable.First(row =>
                row["version"].ToString() == "1.2.0" &&
                row["description"].ToString() == "Third migration" &&
                row["installed_by"].ToString() == "sa");
            Assert.NotEmpty(thirdMigration.Field<string>("checksum")!);
        }
    }
}
