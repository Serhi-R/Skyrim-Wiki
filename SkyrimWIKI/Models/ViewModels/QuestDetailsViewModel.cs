using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

// Details page: the quest itself plus the name of its location.
public class QuestDetailsViewModel
{
    public Quest Quest { get; set; } = null!;
    public string LocationName { get; set; } = string.Empty;
    public string LocationRegion { get; set; } = string.Empty;
}
