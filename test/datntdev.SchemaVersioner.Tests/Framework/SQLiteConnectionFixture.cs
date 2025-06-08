using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace datntdev.SchemaVersioner.Tests.Framework
{
    [TestCaseOrderer("datntdev.SchemaVersioner.Tests.Framework.AlphabeticalOrderer", "datntdev.SchemaVersioner.Tests")]
    public class SQLiteConnectionFixture : IDisposable
    {
        protected readonly Mock<ILogger> _loggerMock = new();

        protected readonly IDbConnection _dbConnection;

        protected virtual string SQLiteConnectionString => "Data Source=sqlite;Mode=Memory;Cache=Shared";

        public SQLiteConnectionFixture()
        {
            _dbConnection = new SqliteConnection(SQLiteConnectionString);
            _dbConnection.Open();
        }

        public SchemaVersioner GetSchemaVersioner(Settings settings)
        {
            return new SchemaVersioner(_dbConnection, _loggerMock.Object, settings);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected DataTable ExecuteQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

        protected void ExecuteNonQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
