using System.Text.Json;

namespace SkyrimWIKI.Services;

// Holds the translation tables in memory. The files live in the Localization
// folder: strings.json (interface, keyed by language) and content.bg.json
// (Bulgarian text for the seeded entities, keyed by category and slug). Loaded
// once and shared, so nothing is read from disk per request.
public sealed class LocalizationStore
{
    public IReadOnlyDictionary<string, string> En { get; }
    public IReadOnlyDictionary<string, string> Bg { get; }
    public IReadOnlyDictionary<string, Dictionary<string, string>> ContentBg { get; }

    public LocalizationStore(IWebHostEnvironment env)
    {
        var dir = Path.Combine(env.ContentRootPath, "Localization");

        var strings = Load<Dictionary<string, Dictionary<string, string>>>(Path.Combine(dir, "strings.json"))
                      ?? new();
        En = strings.TryGetValue("en", out var en) ? en : new Dictionary<string, string>();
        Bg = strings.TryGetValue("bg", out var bg) ? bg : new Dictionary<string, string>();

        ContentBg = Load<Dictionary<string, Dictionary<string, string>>>(Path.Combine(dir, "content.bg.json"))
                    ?? new();
    }

    private static T? Load<T>(string path)
        => File.Exists(path) ? JsonSerializer.Deserialize<T>(File.ReadAllText(path)) : default;
}
