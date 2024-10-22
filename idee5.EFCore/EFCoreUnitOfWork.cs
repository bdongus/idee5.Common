using idee5.Common;
using idee5.Common.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace idee5.EFCore;

/// <summary>
/// The EF core unit of work base implementation.
/// Supports simple concurrency conflict resolution
/// </summary>
/// <typeparam name="TDBContext">Database context type</typeparam>
public abstract class EFCoreUnitOfWork<TDBContext> : IUnitOfWork
    where TDBContext : DbContext {
    protected TDBContext dbContext;
    private bool _disposedValue;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The db context factory to initialize the internally use dbContext field.</param>
    /// <param name="logger">The logger.</param>
    protected EFCoreUnitOfWork(IDbContextFactory<TDBContext> dbContextFactory, ILogger? logger) {
        ArgumentNullException.ThrowIfNull(dbContextFactory);

        dbContext = dbContextFactory.CreateDbContext();
        _logger = logger ?? NullLogger.Instance;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
        bool noConflicts = false;
        while (!noConflicts) {
            try {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                noConflicts = true;
            }
            catch (DbUpdateConcurrencyException ex) {
                foreach (EntityEntry entry in ex.Entries) {
                    string keys = entry.OriginalValues.Properties.Where(p => p.IsPrimaryKey()).Select(p => p.GetGetter().ToString() ?? "").JoinAsString("-");
                    _logger.ConcurrencyConflict(entry.ToString());
                    PropertyValues? databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                    if (databaseValues != null) {
                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    } else {
                        throw new NotSupportedException("No database values for " + entry.Metadata.Name);
                    }
                }
            }
        }
    }
    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                dbContext.Dispose();
            }
            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
