using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models.Configuration;

namespace datntdev.SchemaVersioner.Tests.Loaders
{
    public class MigrationLoader_ShouldLoad
    {
        [Fact]
        public void ShouldLoad_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                MigrationPaths = ["Loaders/Migrations", "Loaders/Repeatable"],
            };
            var loader = new MigrationLoader();

            // Act
            var migrations = loader.Load(settings);

            // Assert
            Assert.Contains(migrations, migration =>
                migration.Type == Models.MigrationType.Versioned
                && migration.Version == "1.0.0"
                && migration.Description == "First migration");
            Assert.Contains(migrations, migration =>
                migration.Type == Models.MigrationType.Versioned
                && migration.Version == "1.1.0"
                && migration.Description == "Second migration");
            Assert.Contains(migrations, migration =>
                migration.Type == Models.MigrationType.Repeatable
                && migration.Version == string.Empty
                && migration.Description == "Repeatable migration");
            Assert.Contains(migrations, migration =>
                migration.Type == Models.MigrationType.Undo
                && migration.Version == "1.0.0"
                && migration.Description == "Undo first migration");
            Assert.Contains(migrations, migration =>
                migration.Type == Models.MigrationType.Undo
                && migration.Version == "1.1.0"
                && migration.Description == "Undo second migration");
        }
    }
}
