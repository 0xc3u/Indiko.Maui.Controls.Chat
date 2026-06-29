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

    // Loads an embedded resource by its file-name suffix (e.g. "sample_video.mp4").
    public static byte[] GetBytesByFileName(string fileName)
    {
        Assembly assembly = typeof(EmbeddedResourceHelper).GetTypeInfo().Assembly;
        string name = assembly.GetManifestResourceNames()
            .First(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
        return GetBytes(name);
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