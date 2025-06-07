namespace datntdev.SchemaVersioner.Interfaces
{
    internal interface IDbEngine
    {
        bool IsMetadataTableExists();
        void CreateMetadataTable();
    }
}
