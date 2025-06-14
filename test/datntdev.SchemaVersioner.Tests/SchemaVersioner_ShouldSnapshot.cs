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
            
            var table1File = snapshotOutputFiles.First(x => x.Contains("T_001__Table1.sql"));
            var table2File = snapshotOutputFiles.First(x => x.Contains("T_002__Table2.sql"));
            var view1File = snapshotOutputFiles.First(x => x.Contains("V_001__View1.sql"));
            var view2File = snapshotOutputFiles.First(x => x.Contains("V_002__View2.sql"));
        }
    }
}
