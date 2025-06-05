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
}
