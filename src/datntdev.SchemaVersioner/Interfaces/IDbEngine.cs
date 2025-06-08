using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IDbEngine
    {
        bool IsMetadataTableExists();
        void CreateMetadataTable();
        void DropMetadataTable();
        Migration[] GetMetadataTable();
        void InsertMigrationRecord(Migration x);
        void EraseDatabase();
    }
}
