using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

// Everything the home page shows: three counters and the latest quests.
public class HomeViewModel
{
    public int QuestCount { get; set; }
    public int LocationCount { get; set; }
    public int RaceCount { get; set; }
    public List<QuestCardViewModel> LatestQuests { get; set; } = [];
}
