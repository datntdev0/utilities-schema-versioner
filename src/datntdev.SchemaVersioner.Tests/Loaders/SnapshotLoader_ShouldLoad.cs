using datntdev.SchemaVersioner.Loaders;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Tests.Loaders
{
    public class SnapshotLoader_ShouldLoad
    {
        [Fact]
        public void ShouldLoad_Successfully()
        {
            // Arrange
            var settings = new Settings()
            {
                SnapshotPaths = ["Loaders/Snapshots"],
            };
            var loader = new SnapshotLoader();
            // Act
            var snapshots = loader.Load(settings);
            // Assert
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.Schema
                && snapshot.Order == "001"
                && snapshot.Description == "Base schema");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.Table
                && snapshot.Order == "001"
                && snapshot.Description == "Table1");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.Table
                && snapshot.Order == "002"
                && snapshot.Description == "Table2");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.View
                && snapshot.Order == "001"
                && snapshot.Description == "View1");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.View
                && snapshot.Order == "002"
                && snapshot.Description == "View2");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.Function
                && snapshot.Order == "001"
                && snapshot.Description == "New function");
            Assert.Contains(snapshots, snapshot =>
                snapshot.Type == Models.SnapshotType.Procedure
                && snapshot.Order == "001"
                && snapshot.Description == "New procedure");
        }
    }
}
