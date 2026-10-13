using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

// A single location and the quests that take place there.
public class LocationDetailsViewModel
{
    public Location Location { get; set; } = null!;
    public List<QuestCardViewModel> Quests { get; set; } = [];
}
