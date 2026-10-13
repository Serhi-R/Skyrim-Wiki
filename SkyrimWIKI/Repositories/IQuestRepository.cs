using SkyrimWIKI.Models;

namespace SkyrimWIKI.Repositories;

// Adds quest specific queries on top of the generic CRUD. The search runs in
// the database: WHERE, ORDER BY, COUNT and the LIMIT/OFFSET of the page.
public interface IQuestRepository : IRepository<Quest>
{
    PagedResult<Quest> Search(QuestFilter filter);
}
