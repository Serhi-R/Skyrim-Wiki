namespace SkyrimWIKI.Services;

// Saves and removes quest images under wwwroot/uploads/quests. Files get a
// random name, so two uploads can never overwrite each other.
public class ImageStorage
{
    public const long MaxBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    // The first bytes of each format. The extension alone can be spoofed, so
    // the content itself has to match too.
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] RiffSignature = [0x52, 0x49, 0x46, 0x46];
    private static readonly byte[] WebpSignature = [0x57, 0x45, 0x42, 0x50];

    private const string Folder = "uploads/quests";

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ImageStorage> _logger;

    public ImageStorage(IWebHostEnvironment env, ILogger<ImageStorage> logger)
    {
        _env = env;
        _logger = logger;
    }

    public bool IsAllowed(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = file.Length is > 0 and <= MaxBytes
            && AllowedExtensions.Contains(extension)
            && HasImageSignature(file, extension);

        if (!allowed)
        {
            _logger.LogWarning(
                "Rejected image upload '{FileName}' of {Length} bytes",
                file.FileName, file.Length);
        }

        return allowed;
    }

    // Reads the header and checks the magic bytes for the claimed format.
    private static bool HasImageSignature(IFormFile file, string extension)
    {
        Span<byte> header = stackalloc byte[12];
        using var stream = file.OpenReadStream();
        var read = stream.Read(header);

        return extension switch
        {
            ".jpg" or ".jpeg" =>
                read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" =>
                read >= PngSignature.Length && header[..PngSignature.Length].SequenceEqual(PngSignature),
            ".webp" =>
                read >= 12 && header[..4].SequenceEqual(RiffSignature) && header[8..12].SequenceEqual(WebpSignature),
            _ => false
        };
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";

        var directory = Path.Combine(_env.WebRootPath, Folder);
        Directory.CreateDirectory(directory);

        await using var stream = File.Create(Path.Combine(directory, fileName));
        await file.CopyToAsync(stream);

        _logger.LogInformation("Saved image {Path} ({Length} bytes)", $"/{Folder}/{fileName}", file.Length);

        return $"/{Folder}/{fileName}";
    }

    // Deletes a file that a previous save created. Paths outside the uploads
    // folder are ignored, so a forged value in the database cannot delete
    // unrelated files.
    public void Delete(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith($"/{Folder}/"))
        {
            return;
        }

        var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted image {Path}", path);
        }
    }
}
