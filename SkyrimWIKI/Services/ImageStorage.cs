namespace SkyrimWIKI.Services;

// Saves and removes quest images under wwwroot/uploads/quests. Files get a
// random name, so two uploads can never overwrite each other.
public class ImageStorage
{
    public const long MaxBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

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
        var allowed = file.Length <= MaxBytes && AllowedExtensions.Contains(extension);

        if (!allowed)
        {
            _logger.LogWarning(
                "Rejected image upload '{FileName}' of {Length} bytes",
                file.FileName, file.Length);
        }

        return allowed;
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
