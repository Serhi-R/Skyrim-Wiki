using Microsoft.EntityFrameworkCore;
using SkyrimWIKI.Data;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Repositories;

// Quest repository with one extra method: a filtered, sorted, paged search.
public class QuestRepository : Repository<Quest>, IQuestRepository
{
    public QuestRepository(AppDbContext context) : base(context)
    {
    }

    public PagedResult<Quest> Search(QuestFilter filter)
    {
        // Location is included because the cards and the API show its name,
        // which avoids a second query per quest.
        var query = DbSet.Include(q => q.Location).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Term))
        {
            var term = filter.Term.Trim();
            query = query.Where(q =>
                EF.Functions.Like(q.Title, $"%{term}%") ||
                EF.Functions.Like(q.Description, $"%{term}%"));
        }

        if (filter.LocationId is { } locationId)
        {
            query = query.Where(q => q.LocationId == locationId);
        }

        if (filter.MinLevel is { } minLevel)
        {
            query = query.Where(q => q.Level >= minLevel);
        }

        if (filter.MaxLevel is { } maxLevel)
        {
            query = query.Where(q => q.Level <= maxLevel);
        }

        query = filter.Sort switch
        {
            "level" => query.OrderBy(q => q.Level).ThenBy(q => q.Title),
            "level_desc" => query.OrderByDescending(q => q.Level).ThenBy(q => q.Title),
            "newest" => query.OrderByDescending(q => q.CreatedAt).ThenBy(q => q.Title),
            "oldest" => query.OrderBy(q => q.CreatedAt).ThenBy(q => q.Title),
            "location" => query.OrderBy(q => q.Location!.Name).ThenBy(q => q.Title),
            _ => query.OrderBy(q => q.Title)
        };

        var totalCount = query.Count();

        // A page number out of range (typed by hand or left over from a filter
        // change) is clamped so the user always sees a page with content.
        var pageSize = filter.PageSize <= 0 ? QuestFilter.DefaultPageSize : filter.PageSize;
        var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var page = Math.Clamp(filter.Page, 1, totalPages);

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Quest>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
