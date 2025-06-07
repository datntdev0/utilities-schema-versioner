using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IDbEngine
    {
        bool IsMetadataTableExists();
        void CreateMetadataTable();
        void InsertMigrationRecord(Migration x);
    }
}
