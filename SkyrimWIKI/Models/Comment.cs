using System.ComponentModel.DataAnnotations;

namespace SkyrimWIKI.Models;

// A comment with a star rating, written by a logged-in user. Belongs to one
// quest; a quest has many comments (1:N).
public class Comment : BaseEntity
{
    [Required]
    [StringLength(1000)]
    [Display(Name = "Comment")]
    public string Text { get; set; } = string.Empty;

    [Range(1, 5)]
    [Display(Name = "Rating")]
    public int Rating { get; set; } = 5;

    // The display name is copied from the account at submit time, so reading
    // comments never needs a join with the Identity tables.
    [Required]
    [StringLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    // The account id, kept so that a user can delete only their own comments.
    [StringLength(450)]
    public string? AuthorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int QuestId { get; set; }
    public Quest? Quest { get; set; }
}
