using idee5.Common;
using idee5.Common.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Linq.Expressions;

namespace idee5.EFCore;
/// <summary>
/// An EF core auditing repository base implementation
/// </summary>
/// <inheritdoc cref="IEntity{TPrimaryKey}" path="/typeparam[@name='TPrimaryKey']"/>
/// <typeparam name="TEntity">Type of the auited entity</typeparam>
/// <typeparam name="TContext">Database context type</typeparam>
public abstract class EFCoreAuditingRepository<TEntity, TPrimaryKey, TContext> : AuditingRepository<TEntity, TPrimaryKey>
    where TContext : DbContext
    where TEntity : class, IAuditedEntity, IEntity<TPrimaryKey>
    where TPrimaryKey : notnull {
    protected readonly TContext dbContext;

    /// <summary>
    /// Repository supporting <see cref="IAuditedEntity" />.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> to use in this repository</param>
    /// <param name="timeProvider">Date and time provider</param>
    /// <param name="currentUserProvider">The current user provider</param>
    protected EFCoreAuditingRepository(TContext dbContext, ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider) : base(timeProvider, currentUserProvider) {
        this.dbContext = dbContext;
    }
    /// <inheritdoc/>
    public override void Add(TEntity item) {
        base.Add(item);

        dbContext.Add(item);
    }
    /// <inheritdoc/>
    public override void Update(TEntity item) {
        // set the auditing properties
        base.Update(item);
        EntityEntry<TEntity>? entry = GetEntity(item);
        if (entry == null) {
            // the item isn't tracked yet, start tracking it
            dbContext.Update(item);
        } else if (entry.State != EntityState.Deleted) {
            // the item is tracked and not deleted, update the non-key properties
            entry.CurrentValues.SetValues(item);
        }
    }

    /// <inheritdoc/>
    public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
        return dbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task ExecuteAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> action, CancellationToken cancellationToken = default) {
        TEntity[] items = await dbContext.Set<TEntity>().Where(predicate).ToArrayAsync(cancellationToken).ConfigureAwait(false);
        if (items?.Length > 0) {
            items.ForEach(action);
        }
    }

    /// <inheritdoc/>
    public override Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
        return dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) {
        return dbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
        return dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
        return dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc/>
    public override void Remove(TEntity item) {
        ArgumentNullException.ThrowIfNull(item);
        TEntity itemToRemove = GetEntity(item)?.Entity ?? item;
        dbContext.Remove(itemToRemove);
    }
    /// <inheritdoc/>
    public override Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(predicate);
        return dbContext.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }
    /// <summary>
    /// Retrieve the change tracking entry by its id fields.
    /// </summary>
    /// <param name="item">The item to look up</param>
    /// <returns>NULL if the item isn't tracked yet.</returns>
    private EntityEntry<TEntity>? GetEntity(TEntity item) => dbContext.ChangeTracker.Entries<TEntity>()
        .SingleOrDefault(ee => ee.Entity.Id.Equals(item.Id));
}
