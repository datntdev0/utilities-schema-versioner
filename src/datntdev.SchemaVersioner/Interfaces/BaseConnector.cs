using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseConnector(SchemaVersionerContext context)
    {
        protected readonly ILogger _logger = context.Logger;

        public IDbConnection DbConnection => context.DbConnection;

        protected abstract string SQL_CheckVersion { get; }
        protected abstract string SQL_GetVersion { get; }

        public bool IsSupported()
        {
            try
            {
                return Execute(SQL_CheckVersion, cmd => Convert.ToInt32(cmd.ExecuteScalar()) == 1);
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

        public string GetVersion() => Execute(SQL_GetVersion, cmd => cmd.ExecuteScalar().ToString());

        public TResult ExecuteScalar<TResult>(string sql)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));

            OpenConnection();
            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            var result = (TResult)cmd.ExecuteScalar();

            CloseConnection();
            return result;
        }

        public TResult Execute<TResult>(string sql, Func<IDbCommand, TResult> query)
        {
            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            ArgumentNullHelper.ThrowIfNull(query, nameof(query));

            OpenConnection();

            using var cmd = DbConnection.CreateCommand();
            cmd.CommandText = sql;

            var result = query(cmd);

            CloseConnection();

            return result;
        }

        protected void OpenConnection()
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
                _logger.LogError("Failed to open database connection: {0}", context.DbConnection.ConnectionString);
                throw new ApplicationException("Failed to open database connection.");
            }
        }

        protected void CloseConnection()
        {
            if (DbConnection.State == ConnectionState.Open)
            {
                DbConnection.Close();
            }
        }

    }
}
