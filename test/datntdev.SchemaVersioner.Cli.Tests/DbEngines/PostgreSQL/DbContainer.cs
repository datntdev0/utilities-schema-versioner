using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.PostgreSQL
{
    public class DbContainer : DockerDbContainer
    {
        private const string DatabaseName = "postgres-database";
        private const string Password = "Password12!";

        private const string DefaultConnectionString = $"Server=127.0.0.1;Database=postgres;User Id=postgres;Password={Password}";

        public override string ConnectionString => $"Server=127.0.0.1;Database={DatabaseName};User Id=postgres;Password={Password}";

        public override string ContainerName => "schema-versioner-postgres";

        public override string ContainerImage => "postgres:latest";

        public override string ContainerExposePort => "5432";

        public override string ContainerHostPort => "5432";

        public override string[] ContainerEnv => [$"POSTGRES_PASSWORD={Password}"];

        public DbContainer()
        {
            DbConnection = new NpgsqlConnection(ConnectionString);
        }

        public override async Task WaitConnection()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    using var connection = new NpgsqlConnection(DefaultConnectionString);
                    await connection.OpenAsync();

                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = $@"CREATE DATABASE ""{DatabaseName}""";
                    cmd.ExecuteNonQuery();

                    await connection.CloseAsync();

                    DbConnection.Open();
                    return;
                }
                catch (NpgsqlException)
                {
                    await Task.Delay(1000);
                }
            }
            throw new TimeoutException("Failed to connect to the database within the timeout period.");
        }
    }
}

