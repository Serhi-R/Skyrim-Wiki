using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkyrimWIKI.Data;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Repositories;

// The one implementation of IRepository<T> that serves every entity type.
// Registered in Program.cs as an open generic:
// AddScoped(typeof(IRepository<>), typeof(Repository<>)).
// Not sealed on purpose: a derived repository can add entity-specific queries.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    // Protected so a derived repository can write more complex queries.
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    // Both the context and the repository are scoped: one instance per HTTP
    // request, so everything in a request shares one change tracker and one
    // SaveChanges.
    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    // Find looks in the change tracker first and queries the database only if
    // the entity is not already loaded.
    public virtual T? GetById(int id) => DbSet.Find(id);

    // ToList() executes here. Without it we would return a query that might be
    // run later, after the context has been disposed at the end of the request.
    public virtual IEnumerable<T> GetAll() => DbSet.ToList();

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => DbSet.Where(predicate).ToList();

    // Marks every property as modified - what you want after model binding has
    // produced a detached object in an Edit action.
    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Add(T entity) => DbSet.Add(entity);

    public virtual void Remove(T entity) => DbSet.Remove(entity);

    // Saves all pending changes on the shared context, not just the ones made
    // through this repository - the unit-of-work idea. Several repositories in
    // one request commit together.
    public virtual void SaveChanges() => Context.SaveChanges();
}
