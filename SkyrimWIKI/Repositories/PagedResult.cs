namespace SkyrimWIKI.Repositories;

// One page of results plus the numbers the view needs to draw pagination.
public class PagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    // At least one page, so an empty result still shows "page 1 of 1".
    public int TotalPages => TotalCount == 0
        ? 1
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
