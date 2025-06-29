# datntdev - SchemaVersioner

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=datntdev0_utilities-schema-versioner&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=datntdev0_utilities-schema-versioner) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=datntdev0_utilities-schema-versioner&metric=coverage)](https://sonarcloud.io/summary/new_code?id=datntdev0_utilities-schema-versioner) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=datntdev0_utilities-schema-versioner&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=datntdev0_utilities-schema-versioner)

`datntdev.SchemaVersioner` provides a robust solution for database schema management, ensuring that changes are applied consistently across different environments. It supports versioned migrations to track incremental changes and repeatable migrations for objects like views or stored procedures that might be updated frequently. The tool is designed to work on various database engines.

## Core Components

- **`datntdev.SchemaVersioner`**: The core class library (found in [`src/datntdev.SchemaVersioner/`](src/datntdev.SchemaVersioner/)) containing the logic for discovering, tracking, and applying database migrations. This package can be installed into your .NET Application which provides a programmatically approach to migrate your databases from your application.
- **`datntdev.SchemaVersioner.Cli`**: A command-line interface (CLI) tool (found in [`src/datntdev.SchemaVersioner.Cli/`](src/datntdev.SchemaVersioner.Cli/)) that utilizes the core library to perform schema versioning operations. This allows for easy integration into build and deployment pipelines.

## Key Features

- **Initialize New Database**: Set up a new database using snapshot scripts.
- **Erase Database**: Completely clear a database (use with extreme caution).
- **Validate Migrations**: Check consistency between migration scripts and metadata.
- **Repair Metadata**: Reconcile the metadata table based on available migration scripts.
- **Targeted Versioning**: Upgrade or downgrade to a specific database version.
- **Migrate to Latest**: Apply all pending migrations to bring the database to the most recent version.
- **Schema Snapshot Generation**: Create scripts representing the current schema for supported databases.
- **CLI Tool for CI/CD Pipeline**: Easily script and automate database migration processes.

### Supported Database Engines

Database | Initialize Command | Erase Command | Validate Command | Repair Command | Upgrade Command | Downgrade Command | Snapshot Command
--- | --- | --- | --- | --- | --- | --- | ---
SQLite | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅
MsSQL | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅
Ms Fabric | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅
PostgreSQL | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌

## Getting Started

### Prerequisites

- For development with Nuget Package: .NET SDK 8.0 or later
- For CLI tool usage: Windows - 64bit

### Installation

- **CLI Tool**: Download the appropriate version for your platform from latest [release](https://github.com/datntdev0/utilities-schema-versioner/releases)

- **Nuget Package**: For using the library in your project
    ```sh
    dotnet add package datntdev.SchemaVersioner
    ```

### Getting Started with CLI

A simple example of SQLite database which init a new database with a snapshot script. This command will initialize the database with the schema defined in the snapshot scripts located in `Resources/SQLite/Snapshots` and apply any migrations found in `Resources/SQLite/Migrations` as a metadata table to avoid running all migration versions for the next time of upgrade version.

```bash
datntdev.SchemaVersioner.Cli.exe --database-type=sqlite ^
--connection-string="Data Source=Resources/SQLite/database.db;Cache=Shared" ^
--migration-paths="Resources/SQLite/Migrations" ^
--snapshot-paths="Resources/SQLite/Snapshots" ^
init
```

### Getting Started with Library

To use the `datntdev.SchemaVersioner` library in your .NET application, you can follow these steps:

```csharp
try
{
    var logger = LoggerFactory.Create(builder => builder.AddSimpleConsole()).CreateLogger<SchemaVersioner>();
    var dbConnection = new SqliteConnection(SQLiteConnectionString);
    var versioner = new SchemaVersioner(dbConnection, logger,
        new SchemaVersionerOptions
        {
            MigrationPaths = new[] { "Resources/SQLite/Migrations" },
            SnapshotPaths = new[] { "Resources/SQLite/Snapshots" }
        });
    // Initialize a new database with snapshot scripts
    versioner.Init();
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    // Handle exceptions as needed
}
```

## Concepts

TBD: Detailed explanation of concepts like versioned migrations, repeatable migrations, and how they are applied in different database engines.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for full details.
