using System;

namespace idee5.Common.Data;
/// <summary>
/// Abstract calls for audited entity repository.
/// </summary>
/// <typeparam name="T"><see cref="Type"/> of the entity.</typeparam>
/// <typeparam name="TKey"><see cref="Type"/> of the primary key</typeparam>
public abstract class AuditingRepository<T, TKey> : ARepository<T, TKey>
    where T : class, IAuditedEntity, IEntity<TKey>
    where TKey : notnull {
    #region Protected Fields

    /// <summary>
    /// User id provider to fill <see cref="IAuditedEntity.CreatedBy"/> and <see cref="IAuditedEntity.ModifiedBy"/>.
    /// </summary>
    protected ICurrentUserIdProvider CurrentUserProvider { get; }

    /// <summary>
    /// Date and time provider to fill <see cref="IAuditedEntity.DateCreatedUTC"/> and <see cref="IAuditedEntity.DateModifiedUTC"/>.
    /// </summary>
    protected ITimeProvider TimeProvider { get; }

    #endregion Protected Fields

    #region Public Constructors

    /// <summary>
    /// Repository supporting <see cref="IAuditedEntity"/>.
    /// </summary>
    /// <param name="timeProvider">Date and time provider. Can be a <see cref="AmbientTimeProvider"/>.</param>
    /// <param name="currentUserProvider"></param>
    protected AuditingRepository(ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider) {
        TimeProvider = timeProvider;
        CurrentUserProvider = currentUserProvider;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Set <see cref="T.DateCreatedUTC"/> and <see cref="T.CreatedBy"/>.
    /// </summary>
    /// <param name="item">The new item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
    public override void Add(T item) {
#if NETSTANDARD2_0_OR_GREATER
        if (item == null) throw new ArgumentNullException(nameof(item));
#else
        ArgumentNullException.ThrowIfNull(item);
#endif
        item.DateCreatedUTC = TimeProvider.UtcNow;
        item.CreatedBy = CurrentUserProvider.GetCurrentUserId();
    }

    /// <summary>
    /// Set <see cref="T.DateModifiedUTC"/> and <see cref="T.ModifiedBy"/>.
    /// <c>base.Update(item);</c>
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
    public override void Update(T item) {
#if NETSTANDARD2_0_OR_GREATER
        if (item == null) throw new ArgumentNullException(nameof(item));
#else
        ArgumentNullException.ThrowIfNull(item);
#endif
        item.DateModifiedUTC = TimeProvider.UtcNow;
        item.ModifiedBy = CurrentUserProvider.GetCurrentUserId();
    }
    #endregion Public Methods
}