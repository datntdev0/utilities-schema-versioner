using datntdev.SchemaVersioner.Helpers;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.Infrastructure
{
    [TestCaseOrderer("datntdev.SchemaVersioner.Tests.Framework.AlphabeticalOrderer", "datntdev.SchemaVersioner.Tests")]
    public class DockerConnectionFixture<TContainer>(TContainer container) : IDisposable
        where TContainer : DockerDbContainer, new()
    {
        protected readonly TContainer _container = container;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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

        protected void ExecuteNonQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            using var cmd = _container.DbConnection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
