namespace datntdev.SchemaVersioner.Models
{
    internal enum CommandType
    {
        Info = 0,
        Init = 1,
        Upgrade = 2,
        Downgrade = 3,
        Validate = 4,
        Repair = 5,
        Snapshot = 6,
    }

    internal enum MigrationType
    {
        None = 0,
        Versioned = 1,
        Undo = 2,
        Repeatable = 3,
    }

    internal enum SnapshotType
    {
        None = 0,
        Schema = 1,
        Table = 2,
        View = 3,
        Procedure = 4,
        Function = 5,
    }

    public enum DbEngineType
    {
        None,
        MsSQL,
        MySQL,
        SQLite,
        Oracle,
        Snowflake,
        PostgreSQL,
        FabricWarehouse,
    }
}
