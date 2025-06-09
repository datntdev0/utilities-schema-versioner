namespace datntdev.SchemaVersioner.Models
{
    internal static class Constants
    {
        internal static class Prefixes
        {
            public const string MigrationVersioned = "V";
            public const string MigrationUndo = "U";
            public const string MigrationRepeatable = "R";

            public const string SnapshotSchema = "S";
            public const string SnapshotTable = "T";
            public const string SnapshotView = "V";
            public const string SnapshotFunction = "F";
            public const string SnapshotProcedure = "P";
        }

        public const string MigrationFileExtension = ".sql";
        public const string MigrationFilePattern = "*.sql";
        public const string ChecksumAlgorithm = "SHA256";
    }
}
