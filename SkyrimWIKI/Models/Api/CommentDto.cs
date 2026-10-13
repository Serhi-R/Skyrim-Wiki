namespace SkyrimWIKI.Models.Api;

// A comment as the front end sees it. Entities are never sent directly, so the
// JSON keeps its shape even if the database changes.
public class CommentDto
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }

    // True when the current user may delete this comment (it is their own).
    public bool CanDelete { get; set; }
}
