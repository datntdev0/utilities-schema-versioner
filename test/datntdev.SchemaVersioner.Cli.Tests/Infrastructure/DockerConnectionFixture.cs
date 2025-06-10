using datntdev.SchemaVersioner.Helpers;
using System.Data;
using System.Data.Common;

namespace datntdev.SchemaVersioner.Cli.Tests.Infrastructure
{
    public class DockerConnectionFixture<TContainer> : IAsyncLifetime
        where TContainer : DockerDbContainer, new()
    {
        protected readonly TContainer _container = new();

        public Task DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await _container.BuildAndStart();
            await WaitConnection();
        }

        protected async Task WaitConnection()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    _container.DbConnection.Open();
                    return; // Connection successful
                }
                catch (Exception)
                {
                    await Task.Delay(1000); // Wait before retrying
                }
            }
            throw new TimeoutException($"Could not connect to the database within {timeout.TotalSeconds} seconds.");
        }

        protected DataTable ExecuteQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
           
            using var cmd = _container.DbConnection.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

    }
}
