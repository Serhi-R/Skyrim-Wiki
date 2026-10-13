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
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Must be called first: it configures the Identity tables.
        base.OnModelCreating(builder);

        // The relations are recognised by convention (LocationId + Location),
        // so only indexes are configured here.
        builder.Entity<Quest>()
            .HasIndex(q => q.Title);

        builder.Entity<Location>()
            .HasIndex(l => l.Name)
            .IsUnique();

        // Comments are always read and counted per quest.
        builder.Entity<Comment>()
            .HasIndex(c => c.QuestId);
    }
}
