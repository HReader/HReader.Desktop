using System;
using System.IO;
using System.Reflection;

namespace HReader.Utility
{
    internal static class Directories
    {
        public static string Executable => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? Environment.CurrentDirectory;
        public static string Sources => Path.Combine(Executable, "sources");
        public static string NativateData => Path.Combine(Executable, "repository");
    }

    internal static class Files
    {
        public static string MetadataRepository => Path.Combine(Directories.Executable, "repository.db");
    }
}
