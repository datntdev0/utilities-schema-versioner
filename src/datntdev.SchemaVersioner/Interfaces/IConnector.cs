using datntdev.SchemaVersioner.Models;
namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IConnector
    {
        DbEngineType DbEngineType { get; }

        string GetVersion();
        bool IsSupported();
    }
}
