using datntdev.SchemaVersioner.Models;
using System.Data;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IConnector
    {
        IDbConnection DbConnection { get; }
        DbEngineType DbEngineType { get; }

        string GetVersion();
        bool IsSupported();
    }
}
