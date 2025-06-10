using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsSQL
{
    public class DbContainer : DockerDbContainer
    {
        public override string ConnectionString => "Server=127.0.0.1;Database=master;User Id=sa;Password=Password12!;TrustServerCertificate=True";

        public override string ContainerName => "schema-versioner-mssql";

        public override string ContainerImage => "mcr.microsoft.com/mssql/server:latest";

        public override string ContainerExposePort => "1433";

        public override string ContainerHostPort => "1433";

        public override string[] ContainerEnv => ["ACCEPT_EULA=Y", "MSSQL_SA_PASSWORD=Password12!"];

        public override IDbConnection DbConnection { get; protected set; }

        public DbContainer()
        {
            DbConnection = new SqlConnection(ConnectionString);
        }
    }
}
