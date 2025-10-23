// Brings in EF Core types like DbContext, DbSet, ModelBuilder, etc.
using Microsoft.EntityFrameworkCore;
// Lets this file see the EventRecord class we defined in Models/
using TimezoneApi.Models;

// Namespace (folder/package) for data-access layer classes.
// Conventionally we put DbContext inside a ".Data" namespace.
namespace TimezoneApi.Data;

public class AppDbContext : DbContext   // DbContext = EF Core's "unit of work" + change tracker
{
    // Exposes a typed "table" for EventRecord.
    // DbSet<T> lets you query/write rows; EF maps it to the real table.
    public DbSet<EventRecord> Events => Set<EventRecord>();

    // Constructor: EF will inject DbContextOptions with the connection string/provider (UseNpgsql).
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    // Called by EF when building the model. We configure table/schema/column mappings here.
    protected override void OnModelCreating(ModelBuilder b)
    {
        // Default schema for all mapped tables is "app" (not the default "public").
        b.HasDefaultSchema("app");

        // Configure the EventRecord entity → which table and columns it maps to.
        b.Entity<EventRecord>(e =>
        {
            // Map EventRecord to the table named "events".
            e.ToTable("events");

            // Map C# property Id → column "id" (uuid).
            e.Property(p => p.Id).HasColumnName("id");

            // Map Title → "title" (text).
            e.Property(p => p.Title).HasColumnName("title");

            // Map SavedUtc → "saved_utc" (timestamptz, always UTC).
            e.Property(p => p.SavedUtc).HasColumnName("saved_utc");

            // Map SavedTz → "saved_tz" (text like "Australia/Sydney").
            e.Property(p => p.SavedTz).HasColumnName("saved_tz");
        });
    }
}
