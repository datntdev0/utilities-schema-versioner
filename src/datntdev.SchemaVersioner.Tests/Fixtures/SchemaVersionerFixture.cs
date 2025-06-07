using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace datntdev.SchemaVersioner.Tests.Fixtures
{
    public class SchemaVersionerFixture : IDisposable
    {
        protected readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        protected readonly IDbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

        public SchemaVersioner GetSchemaVersioner(Settings settings)
        {
            return new SchemaVersioner(_dbConnection, _loggerMock.Object, settings);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected TResult? ExecuteScalar<TResult>(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;
            var result = (TResult?)cmd.ExecuteScalar();
            return result;
        }

        protected DataTable ExecuteReader(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }
}
