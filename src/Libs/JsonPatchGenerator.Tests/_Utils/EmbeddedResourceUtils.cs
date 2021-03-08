using System;
using System.IO;
using System.Reflection;

namespace JsonPatchGenerator.Tests._Utils
{
    public static class EmbeddedResourceUtils
    {
        public static string LoadResourceFile(Assembly assembly, string path)
        {
            using var stream = assembly.GetManifestResourceStream(path);

            if (stream == null) throw new InvalidOperationException("Could not load manifest resource stream.");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}