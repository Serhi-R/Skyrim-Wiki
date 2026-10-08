namespace SkyrimWIKI.Models;

// Base class for every entity stored in the database. The Id here is what
// makes the generic repository possible: IRepository<T> requires T :
// BaseEntity, which promises the compiler that every T has an Id, so GetById
// can be written once for all entities.
public abstract class BaseEntity
{
    // EF Core recognises a property named Id by convention and configures it
    // as an auto-incrementing primary key.
    public int Id { get; set; }
}
