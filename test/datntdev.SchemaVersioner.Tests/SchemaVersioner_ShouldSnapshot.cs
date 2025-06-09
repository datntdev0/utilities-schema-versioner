using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldSnapshot : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=sqlite_snapshot;Mode=Memory;Cache=Shared";

        [Fact]
        public void ShouldSnapshot_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                MigrationPaths = ["Resources/SQLite/Migrations"],
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                SnapshotOutputPath = "Resources/SQLite/SnapshotsOutput"
            };
            GetSchemaVersioner(settings).Init();

            // Act
            var output = GetSchemaVersioner(settings).Snapshot();

            // Assert
            Assert.NotNull(output.Data);

            var snapshotOutputFiles = Directory.GetFiles(
                settings.SnapshotOutputPath, "*.sql", SearchOption.AllDirectories);
            Assert.Equal(4, snapshotOutputFiles.Length);
            
            var table1File = snapshotOutputFiles.First(x => x.Contains("T__001_Table1.sql"));
            var table2File = snapshotOutputFiles.First(x => x.Contains("T__002_Table2.sql"));
            var view1File = snapshotOutputFiles.First(x => x.Contains("V__001_View1.sql"));
            var view2File = snapshotOutputFiles.First(x => x.Contains("V__002_View2.sql"));

            Assert.Equal(@"CREATE TABLE [Table1] (
	""Id"" INTEGER PRIMARY KEY AUTOINCREMENT,
	""Name"" TEXT NOT NULL,
	""CreatedAt"" DATETIME DEFAULT CURRENT_TIMESTAMP
)", File.ReadAllText(table1File));

            Assert.Equal(@"CREATE TABLE [Table2] (
	""Id"" INTEGER PRIMARY KEY AUTOINCREMENT,
	""Name"" TEXT NOT NULL,
	""CreatedAt"" DATETIME DEFAULT CURRENT_TIMESTAMP
)", File.ReadAllText(table2File));

            Assert.Equal(@"CREATE VIEW [View1] AS
SELECT ""Id"", ""Name"" [Table1]", File.ReadAllText(view1File));

            Assert.Equal(@"CREATE VIEW [View2] AS
SELECT ""Id"", ""Name"" [Table2]", File.ReadAllText(view2File));
        }
    }
}
