using System.Text;

namespace SkyrimWIKI.Services;

// Looks up the picture for an entity and always hands back something usable.
//
// Priority for every kind of picture:
//   1. a game image dropped into wwwroot/img/game/<category>/<slug>
//   2. the built-in fallback (a Wikimedia photo for holds, the uploaded file
//      for quests, nothing otherwise)
//
// A single file drop is enough: the picture may be named with the size suffix
// the optimiser writes (-800 / -1600) or just <slug>.<ext>. When nothing is
// found the caller gets null and the view renders no <img>, so there are never
// broken images or 404s.
public sealed class GameImages
{
    private static readonly string[] Sizes = ["800", "1600"];

    private readonly string _root;

    public GameImages(IWebHostEnvironment env) => _root = env.WebRootPath;

    // "The Woodsman's Friend" -> "the-woodsman-s-friend". Same rule is written
    // down in docs/IMAGES-NEEDED.md, so file names and the resolver always agree.
    public static string Slug(string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var ch in value.Trim().ToLowerInvariant())
        {
            if (ch is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                sb.Append(ch);
            }
            else if (sb.Length > 0 && sb[^1] != '-')
            {
                sb.Append('-');
            }
        }

        return sb.ToString().Trim('-');
    }

    // The home hero. Always resolves, the Wikimedia photo is the last resort.
    public string Hero()
        => Game("hero", "home", "1600") ?? "/img/hero-1600.webp";

    public string? Race(string name, bool large = false)
        => Game("races", name, large ? "1600" : "800");

    public string? Hold(string name, bool large = false)
        => Game("holds", name, large ? "1600" : "800")
           ?? (large ? HoldImages.Full(name) : HoldImages.For(name));

    public string? Quest(string title, string? uploadedPath = null, bool large = false)
        => Game("quests", title, large ? "1600" : "800") ?? uploadedPath;

    // Returns a web-root relative url ("/img/game/...") or null when no file
    // matches. Tries the optimised size suffixes first, then the raw drop.
    public string? Game(string category, string name, string? size = "800")
    {
        var slug = Slug(name);
        if (slug.Length == 0)
        {
            return null;
        }

        foreach (var candidate in Candidates(category, slug, size))
        {
            var full = Path.Combine(_root, candidate.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(full))
            {
                return "/" + candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> Candidates(string category, string slug, string? size)
    {
        var folder = $"img/game/{category}/{slug}";
        if (size is not null)
        {
            yield return $"{folder}-{size}.webp";
        }

        foreach (var other in Sizes)
        {
            if (other != size)
            {
                yield return $"{folder}-{other}.webp";
            }
        }

        yield return $"{folder}.webp";
        yield return $"{folder}.jpg";
        yield return $"{folder}.jpeg";
        yield return $"{folder}.png";
    }
}
