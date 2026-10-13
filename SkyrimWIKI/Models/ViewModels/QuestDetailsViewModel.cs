using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Models.ViewModels;

// Details page: the quest itself, its location and the comments left by users.
public class QuestDetailsViewModel
{
    public Quest Quest { get; set; } = null!;
    public string LocationName { get; set; } = string.Empty;
    public string LocationRegion { get; set; } = string.Empty;
    public List<Comment> Comments { get; set; } = [];
    public double AverageRating { get; set; }
}
