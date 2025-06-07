using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class SnapshotCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Scan and generate DDL for all procedures

            // Scan and generate DDL for all functions

            // Scan and generate DDL for all views

            // Scan and generate DDL for all tables

            // Scan and generate DDL for all schemas

            // Create sql scripts for that all DDL

            return new CommandOutput<CommandOutputSnapshot>(new CommandOutputSnapshot());
        }
    }
}
