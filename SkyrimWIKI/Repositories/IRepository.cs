using System.Linq.Expressions;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Repositories;

// The only way controllers are allowed to reach the database.
// Controllers depend on this interface instead of EF Core, so they stay small
// and testable - a test can pass a fake repository.
public interface IRepository<T> where T : BaseEntity
{
    // The entity with the given id, or null. The T? return type forces the
    // caller to handle "not found" instead of hitting a NullReferenceException.
    T? GetById(int id);

    IEnumerable<T> GetAll();

    // Takes an Expression, not a plain Func: an expression is a description
    // EF Core can translate into SQL WHERE, so filtering happens in the
    // database and not in memory.
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

    // Add / Update / Remove only change in-memory state. Nothing is written
    // until SaveChanges is called.
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);

    // Issues the actual INSERT / UPDATE / DELETE in one transaction. Forgetting
    // it is the usual reason "my data is not saved".
    void SaveChanges();
}
