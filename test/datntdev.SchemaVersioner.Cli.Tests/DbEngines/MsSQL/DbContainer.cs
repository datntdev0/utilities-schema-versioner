using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsSQL
{
    public class DbContainer : DockerDbContainer
    {
        private const string DatabaseName = "mssql-database";
        private const string Password = "Password12!";

        private const string MasterConnectionString = $"Server=127.0.0.1;Database=master;User Id=sa;Password={Password};TrustServerCertificate=True";

        public override string ConnectionString => $"Server=127.0.0.1;Database={DatabaseName};User Id=sa;Password={Password};TrustServerCertificate=True";

        public override string ContainerName => "schema-versioner-mssql";

        public override string ContainerImage => "mcr.microsoft.com/mssql/server:2022-latest";

        public override string ContainerExposePort => "1433";

        public override string ContainerHostPort => "1433";

        public override string[] ContainerEnv => ["ACCEPT_EULA=Y", $"MSSQL_SA_PASSWORD={Password}"];

        public DbContainer()
        {
            DbConnection = new SqlConnection(ConnectionString);
        }

        public override async Task WaitConnection()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    using var connection = new SqlConnection(MasterConnectionString);
                    await connection.OpenAsync();

                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE [{DatabaseName}]";
                    cmd.ExecuteNonQuery();

                    await connection.CloseAsync();

                    DbConnection.Open();
                    return;
                }
                catch (SqlException)
                {
                    await Task.Delay(1000);
                }
            }
            throw new TimeoutException("Failed to connect to the database within the timeout period.");
        }
    }
}
