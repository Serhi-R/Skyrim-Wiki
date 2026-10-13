using System.ComponentModel.DataAnnotations;

namespace SkyrimWIKI.Models.Api;

// POST body for a new comment. [ApiController] turns a failed validation into
// an automatic 400 with the error messages.
public class CommentInput
{
    [Range(1, int.MaxValue)]
    public int QuestId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 3)]
    public string Text { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;
}
