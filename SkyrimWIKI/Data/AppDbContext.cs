using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkyrimWIKI.Models;

namespace SkyrimWIKI.Data;

// The whole database as one class. It inherits IdentityDbContext so the
// Identity tables (AspNetUsers, AspNetRoles, ...) come for free.
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // One DbSet per entity: DbSet<T> is the table for T.
    //
    // public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Must be called first: it configures the Identity tables.
        base.OnModelCreating(builder);

        // Keys, indexes and relationships go here.
    }
}
