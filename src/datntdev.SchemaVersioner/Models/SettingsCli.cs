namespace datntdev.SchemaVersioner.Models
{
    public class SettingsCli : Settings
    {
        public DbEngineType DbEngineType { get; set; } = DbEngineType.SQLite;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
