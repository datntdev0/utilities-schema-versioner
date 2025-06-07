using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.DbEngines
{
    internal class MsSQLDbEngine(SchemaVersionerContext context) : BaseDbEngine(context), IDbEngine
    {
        public void CreateMetadataTable()
        {
            throw new System.NotImplementedException();
        }

        public void InsertMigrationRecord(Migration x)
        {
            throw new System.NotImplementedException();
        }

        public bool IsMetadataTableExists()
        {
            throw new System.NotImplementedException();
        }
    }
}
