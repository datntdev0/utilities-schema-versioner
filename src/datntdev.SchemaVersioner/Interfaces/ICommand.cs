using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface ICommand
    {
        CommandOutput Execute();
    }
}
