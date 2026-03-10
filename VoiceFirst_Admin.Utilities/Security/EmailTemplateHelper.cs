using System.Collections.Concurrent;
using System.Reflection;

namespace VoiceFirst_Admin.Utilities.Security;

public static class EmailTemplateHelper
{
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    public static string GetTemplate(string templateName)
    {
        return Cache.GetOrAdd(templateName, static name =>
        {
            var assembly = typeof(EmailTemplateHelper).Assembly;
            var resourceName = $"VoiceFirst_Admin.Utilities.Templates.{name}.html";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded template '{resourceName}' not found.");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        });
    }
}
