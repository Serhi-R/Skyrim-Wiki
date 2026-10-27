namespace SkyrimWIKI.Services;

// Maps a hold name to the licensed landscape photo that stands in for it.
// Only the holds that have a picture are listed; anything else returns null
// and the view falls back to a plain surface.
public static class HoldImages
{
    private static readonly HashSet<string> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        "whiterun", "windhelm", "riften", "solitude",
        "markarth", "falkreath", "dawnstar", "winterhold"
    };

    public static string? For(string locationName)
        => Known.Contains(locationName)
            ? $"/img/{locationName.ToLowerInvariant()}-800.webp"
            : null;

    public static string? Full(string locationName)
        => Known.Contains(locationName)
            ? $"/img/{locationName.ToLowerInvariant()}-1600.webp"
            : null;
}
