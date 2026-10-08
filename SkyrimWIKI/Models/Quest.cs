using System.ComponentModel.DataAnnotations;

namespace SkyrimWIKI.Models;

// A quest entry - the main object of the wiki and the one with full CRUD.
// A quest belongs to one location; a location has many quests (1:N).
public class Quest : BaseEntity
{
    [Required]
    [StringLength(120)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Range(1, 81)]
    [Display(Name = "Recommended level")]
    public int Level { get; set; } = 1;

    [Required]
    [StringLength(300)]
    [Display(Name = "Reward")]
    public string Reward { get; set; } = string.Empty;

    // Relative path under wwwroot, e.g. /uploads/quest-1.png. Null when the
    // quest has no image yet.
    [StringLength(500)]
    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key for the location relation. EF Core wires the two properties
    // up by convention: LocationId + Location.
    public int LocationId { get; set; }
    public Location? Location { get; set; }

    // A quest has many comments (1:N).
    public List<Comment> Comments { get; set; } = [];
}
