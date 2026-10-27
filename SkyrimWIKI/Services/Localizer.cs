namespace SkyrimWIKI.Services;

// Per-request translator. The language comes from the "lang" cookie, so every
// view and controller sees the same choice without threading it around.
// Usage: L["nav.home"] for interface text, L.Content("races", race.Name, ...)
// for seeded content. Unknown keys fall back to English, then to the key.
public sealed class Localizer
{
    public const string CookieName = "lang";

    private readonly LocalizationStore _store;

    public Localizer(LocalizationStore store, IHttpContextAccessor accessor)
    {
        _store = store;
        Lang = accessor.HttpContext?.Request.Cookies[CookieName] == "bg" ? "bg" : "en";
    }

    public string Lang { get; }

    public bool IsBg => Lang == "bg";

    public string this[string key]
    {
        get
        {
            var table = IsBg ? _store.Bg : _store.En;
            if (table.TryGetValue(key, out var value))
            {
                return value;
            }

            return _store.En.TryGetValue(key, out var fallback) ? fallback : key;
        }
    }

    public string Content(string category, string name, string fallback)
    {
        if (IsBg
            && _store.ContentBg.TryGetValue(category, out var map)
            && map.TryGetValue(GameImages.Slug(name), out var value))
        {
            return value;
        }

        return fallback;
    }
}
