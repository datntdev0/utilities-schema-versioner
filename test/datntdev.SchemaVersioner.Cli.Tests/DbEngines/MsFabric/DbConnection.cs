using datntdev.SchemaVersioner.Cli.Tests.Infrastructure;
using Microsoft.Data.SqlClient;

namespace datntdev.SchemaVersioner.Cli.Tests.DbEngines.MsFabric
{
    public class DbConnection : ActualDbConnection
    {
        private const string DatabaseServer = "*.datawarehouse.fabric.microsoft.com";
        private const string DatabaseName = "database_name";
        private const string AuthenticationType = "Active Directory Service Principal";
        private const string ClientId = "";
        private const string ClientSecret = "";

        public override string ConnectionString => $"Server=tcp:{DatabaseServer},1433;Database={DatabaseName};Authentication={AuthenticationType};User Id={ClientId};Password={ClientSecret};Encrypt=True;TrustServerCertificate=False";

        public DbConnection()
        {
            DbConnection = new SqlConnection(ConnectionString);
        }
    }
}
