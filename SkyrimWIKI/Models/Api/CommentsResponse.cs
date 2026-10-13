namespace SkyrimWIKI.Models.Api;

// GET /api/comments?questId=1 - the comments of one quest plus the summary the
// star rating widget needs.
public class CommentsResponse
{
    public int QuestId { get; set; }
    public int Count { get; set; }
    public double AverageRating { get; set; }
    public List<CommentDto> Items { get; set; } = [];
}
