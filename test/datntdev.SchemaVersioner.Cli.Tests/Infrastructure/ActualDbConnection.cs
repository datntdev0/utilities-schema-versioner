using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.Infrastructure
{
    public abstract class ActualDbConnection : IAsyncLifetime
    {
        public IDbConnection DbConnection { get; protected set; } = default!;

        public abstract string ConnectionString { get; }

        public Task DisposeAsync()
        {
            DbConnection?.Dispose();
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            DbConnection.Open();
            return Task.CompletedTask;
        }
    }
}
