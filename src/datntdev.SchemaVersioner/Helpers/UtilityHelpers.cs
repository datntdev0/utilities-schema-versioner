using System;

namespace datntdev.SchemaVersioner.Helpers
{
    internal static class ArgumentNullHelper
    {
        public static void ThrowIfNull(object argument, string name)
        {
            if (argument == null) throw new ArgumentNullException(name);
        }
    }
}
