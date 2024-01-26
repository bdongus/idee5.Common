using idee5.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace idee5.EventStoreLite;

/// <summary>
/// The event store db context.
/// </summary>
public class EventStoreDbContext : DbContext {
    /// <summary>
    /// Default connection string name.
    /// </summary>
    public static readonly string connectionStringName = "EventStoreConnection";

    /// <summary>
    /// Connection string to the database.
    /// </summary>
    protected string connectionString = "";

    /// <summary>
    /// Create a context with the given options.
    /// </summary>
    /// <param name="options"><see cref="DbContextOptions{EventStoreDbContext}"/> created with a <see cref="DbContextOptionsBuilder{EventStoreDbContext}"/>.</param>
    /// <example>
    /// var contextOptions = new DbContextOptionsBuilder&lt;EventStoreDbContext&gt;();
    /// contextOptions.UseSqlite("data source=|DataDirectory|eventstore.db3");
    /// context = new EventStoreDbContext(contextOptions.Options);
    /// </example>

    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options) {

    }

    /// <summary>
    /// Constructor to allow derived contexts to be resolved by Microsoft DI.
    /// </summary>
    /// <remarks>See this github issue: https://github.com/aspnet/EntityFrameworkCore/issues/7533#issuecomment-353669263 </remarks>
    /// <param name="options"></param>
    protected EventStoreDbContext(DbContextOptions options) : base(options) {

    }

    /// <summary>
    /// Gets or Sets the event entries.
    /// </summary>
    public DbSet<EventEntry> EventEntries { get; set; }
    /// <summary>
    /// Gets or Sets the snapshot entries.
    /// </summary>
    public DbSet<SnapshotEntry> SnapshotEntries { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // configure event store
        modelBuilder.Entity<EventEntry>().ToTable("evententry");
        modelBuilder.Entity<EventEntry>().HasIndex(e => new { e.Id, e.Version }).IsUnique();

        // configure snapshot store
        modelBuilder.Entity<SnapshotEntry>().ToTable("snapshotentry");
    }
}
