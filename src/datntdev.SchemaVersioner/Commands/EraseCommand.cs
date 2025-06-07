using datntdev.SchemaVersioner.Interfaces;
using datntdev.SchemaVersioner.Models;

namespace datntdev.SchemaVersioner.Commands
{
    internal class EraseCommand(SchemaVersionerContext context) : BaseCommand(context), ICommand
    {
        public CommandOutput Execute()
        {
            // Scan current database to get list of procedures

            // Scan current database to get list of functions

            // Scan current database to get list of views

            // Scan current database to get list of tables

            // Scan current database to get list of schemas

            // Drop all procedures, functions, views, tables, and schemas

            return new CommandOutput<CommandOutputErase>(new CommandOutputErase());
        }
    }
}
