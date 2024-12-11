using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Indiko.Maui.Controls.Chat.Sample.Utils;
public static class EmbeddedResourceHelper
{
    public static IEnumerable<string> GetAvailableImages()
    {
        Assembly assembly = typeof(EmbeddedResourceHelper).GetTypeInfo().Assembly;
        return assembly.GetManifestResourceNames().Where(e => e.Contains(".png"));
    }

    public static byte[] GetBytes(string resourceName)
    {
        Assembly assembly = typeof(EmbeddedResourceHelper).GetTypeInfo().Assembly;

        using Stream stream = assembly.GetManifestResourceStream(resourceName) 
            ?? throw new InvalidOperationException($"Resource {resourceName} not found.");
        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}