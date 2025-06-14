using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseConnector : IDisposable
    {
        public readonly IDbConnection DbConnection;

        protected readonly ILogger _logger;

        protected abstract string SQL_CheckVersion { get; }
        protected abstract string SQL_GetVersion { get; }

        public BaseConnector(SchemaVersionerContext context)
        {
            _logger = context.Logger;
            DbConnection = context.DbConnection;
            OpenConnection();
        }

        public bool IsSupported()
        {
            try
            {
                return ExecuteScalar<long>(SQL_CheckVersion) == 1;
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetVersion() => ExecuteScalar<string>(SQL_GetVersion);

        public DataTable ExecuteQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public virtual void ExecuteComplexContent(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
        }

        public TResult ExecuteScalar<TResult>(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            var result = (TResult)cmd.ExecuteScalar()!;

            return result;
        }

        public TResult Execute<TResult>(string sql, Func<IDbCommand, TResult> query)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            ArgumentNullHelper.ThrowIfNull(query, nameof(query));

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            var result = query(cmd);

            return result;
        }

        public void OpenConnection()
        {
            try
            {
                if (DbConnection.State == ConnectionState.Broken)
                {
                    DbConnection.Close();
                }

                if (DbConnection.State != ConnectionState.Open)
                {
                    DbConnection.Open();
                }
            }
            catch (Exception)
            {
                _logger.LogError("Failed to open database connection: {0}", DbConnection.ConnectionString);
                throw new ApplicationException("Failed to open database connection.");
            }
        }

        public void CloseConnection()
        {
            if (DbConnection.State == ConnectionState.Open)
            {
                DbConnection.Close();
            }
        }

        public void Dispose()
        {
            DbConnection.Dispose();
        }
    }
}
