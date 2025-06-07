using datntdev.SchemaVersioner.Helpers;
using datntdev.SchemaVersioner.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal abstract class BaseConnector(ILogger logger, IDbConnection dbConnection) : IConnector
    {
        protected readonly ILogger _logger = logger;
        protected readonly IDbConnection _dbConnection = dbConnection;

        protected abstract DbEngineType DbEngineType { get; }
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

        public string GetVersion()
        {
            return Execute(SQL_GetVersion, cmd => cmd.ExecuteScalar().ToString());
        }

        protected TResult Execute<TResult>(string sql, Func<IDbCommand, TResult> query)
        {
            OpenConnection();

            ArgumentNullHelper.ThrowIfNull(sql, nameof(sql));
            ArgumentNullHelper.ThrowIfNull(query, nameof(query));

            using var cmd = _dbConnection.CreateCommand();
            cmd.CommandText = sql;

            var result = query(cmd);

            CloseConnection();

            return result;
        }

        protected void OpenConnection()
        {
            try
            {
                if (_dbConnection.State == ConnectionState.Broken)
                {
                    _dbConnection.Close();
                }

                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed to open database connection.");
            }
        }

        protected void CloseConnection()
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
            }
        }
    }
}
