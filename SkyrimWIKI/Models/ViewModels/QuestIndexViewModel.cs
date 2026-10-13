using SkyrimWIKI.Models;
using SkyrimWIKI.Repositories;

namespace SkyrimWIKI.Models.ViewModels;

// The quest list page: current filter, the page of cards and the data the
// filter form and pagination need.
public class QuestIndexViewModel
{
    public QuestFilter Filter { get; set; } = new();
    public List<QuestCardViewModel> Quests { get; set; } = [];
    public List<Location> Locations { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => Filter.Page > 1;
    public bool HasNextPage => Filter.Page < TotalPages;
}
