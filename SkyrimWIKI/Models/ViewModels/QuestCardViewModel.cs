using SkyrimWIKI.Models;

namespace SkyrimWIKI.Models.ViewModels;

// Summary shown as a card on the home page and in quest lists.
public class QuestCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Level { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;

    public static QuestCardViewModel From(Quest quest, string locationName)
    {
        var excerpt = quest.Description.Length > 140
            ? quest.Description[..140] + "..."
            : quest.Description;

        return new QuestCardViewModel
        {
            Id = quest.Id,
            Title = quest.Title,
            Level = quest.Level,
            LocationName = locationName,
            Excerpt = excerpt
        };
    }
}
