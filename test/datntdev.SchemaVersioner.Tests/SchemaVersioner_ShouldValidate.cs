using datntdev.SchemaVersioner.Models;
using datntdev.SchemaVersioner.Tests.Framework;

namespace datntdev.SchemaVersioner.Tests
{
    public class SchemaVersioner_ShouldValidate : SQLiteConnectionFixture
    {
        protected override string SQLiteConnectionString => "Data Source=sqlite_validate;Mode=Memory;Cache=Shared";
      
        [Fact]
        public void ShouldValidate_1_RisedException_WhenMetadataTableNotExists()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => GetSchemaVersioner(settings).Validate());
            Assert.Equal("Metadata table does not exist. Please run 'init', 'repair', or 'upgrade' command first.", ex.Message);
        }

        [Fact]
        public void ShouldValidate_2_AllMigrations_AreSuccess()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            GetSchemaVersioner(settings).Repair();

            // Act
            var output = GetSchemaVersioner(settings).Validate();
            // Assert
            Assert.NotNull(output.Data.ResultTable);
            Assert.Contains(output.Data.ResultTable.Rows, row =>
                row[0].ToString() == "1.0.0" &&
                row[1].ToString() == "First migration" &&
                !string.IsNullOrEmpty(row[2].ToString()) && row[5].ToString() == "Yes");
            Assert.Contains(output.Data.ResultTable.Rows, row =>
                row[0].ToString() == "1.1.0" &&
                row[1].ToString() == "Second migration" &&
                !string.IsNullOrEmpty(row[2].ToString()) && row[5].ToString() == "Yes");
        }

        [Fact]
        public void ShouldValidate_3_OneMigration_ChecksumMismatch()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            ExecuteNonQuery("UPDATE schema_versioner_migrations SET checksum = 'invalid_checksum' WHERE version = '1.0.0'");

            // Act
            var output = GetSchemaVersioner(settings).Validate();
            // Assert
            Assert.NotNull(output.Data.ResultTable);
            Assert.Contains(output.Data.ResultTable.Rows, row =>
                row[0].ToString() == "1.0.0" &&
                row[1].ToString() == "First migration" &&
                row[2].ToString() == "Checksum Mismatch" && row[5].ToString() == "N/A");
            Assert.Contains(output.Data.ResultTable.Rows, row =>
                row[0].ToString() == "1.1.0" &&
                row[1].ToString() == "Second migration" &&
                !string.IsNullOrEmpty(row[2].ToString()) && row[5].ToString() == "Yes");
        }

        [Fact]
        public void ShouldValidate_4_AllMigrations_ArePending()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Resources/SQLite/Snapshots"],
                MigrationPaths = ["Resources/SQLite/Migrations"],
            };
            ExecuteNonQuery("DELETE FROM schema_versioner_migrations");

            // Act
            var output = GetSchemaVersioner(settings).Validate();

            // Assert
            Assert.NotNull(output.Data.ResultTable);
            Assert.Contains(output.Data.ResultTable.Rows, row => 
                row[0].ToString() == "1.0.0" &&
                row[1].ToString() == "First migration" &&
                row[2].ToString() == "N/A" && row[3].ToString() == "N/A" && row[5].ToString() == "N/A");
            Assert.Contains(output.Data.ResultTable.Rows, row =>
                row[0].ToString() == "1.1.0" &&
                row[1].ToString() == "Second migration" &&
                row[2].ToString() == "N/A" && row[3].ToString() == "N/A" && row[5].ToString() == "N/A");
        }
    }
}
