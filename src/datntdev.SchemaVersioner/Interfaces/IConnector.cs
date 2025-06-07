using System.Data;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IConnector
    {
        string GetVersion();
        bool IsSupported();
    }
}
