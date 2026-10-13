namespace SkyrimWIKI.Repositories;

// Everything the quest list can be narrowed by. Bound from the query string,
// so the same object serves the MVC page and the JSON API.
public class QuestFilter
{
    public const int DefaultPageSize = 6;

    public string? Term { get; set; }
    public int? LocationId { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
}
