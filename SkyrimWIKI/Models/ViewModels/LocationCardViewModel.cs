using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

// A location card in the locations list together with how many quests it
// holds, so no extra query per row is needed.
public class LocationCardViewModel
{
    public Location Location { get; set; } = null!;
    public int QuestCount { get; set; }
}
