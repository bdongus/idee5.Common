using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data {
    /// <summary>
    /// Abstract calls for audited entity repository.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the entity.</typeparam>
    /// <typeparam name="TKey"><see cref="Type"/> of the primary key</typeparam>
    public abstract class AuditingRepository<T, TKey> : ARepository<T, TKey> where T : class, IAuditedEntity, IEntity<TKey> {
        #region Protected Fields

        /// <summary>
        /// User id provider to fill <see cref="IAuditedEntity.CreatedBy"/> and <see cref="IAuditedEntity.ModifiedBy"/>.
        /// </summary>
        protected ICurrentUserIdProvider CurrentUserProvider { get; }

        /// <summary>
        /// Date and time provider to fill <see cref="IAuditedEntity.DateCreated"/> and <see cref="IAuditedEntity.DateModified"/>.
        /// </summary>
        protected ITimeProvider TimeProvider { get; }

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// Repository supporting <see cref="IAuditedEntity"/>.
        /// </summary>
        /// <param name="timeProvider">Date and time provider. Can be a <see cref="AmbientTimeProvider"/>.</param>
        /// <param name="currentUserProvider"></param>
        protected AuditingRepository(ITimeProvider timeProvider, ICurrentUserIdProvider currentUserProvider)
        {
            TimeProvider = timeProvider;
            CurrentUserProvider = currentUserProvider;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="item">The new item to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public override void Add(T item) {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.DateCreated = TimeProvider.UtcNow;
            item.CreatedBy = CurrentUserProvider.GetCurrentUserId();
        }

        /// <summary>
        /// Updates an item in the reopsitory.
        /// <code>base.Update(item);</code>
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public override void Update(T item) {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.DateModified = TimeProvider.UtcNow;
            item.ModifiedBy = CurrentUserProvider.GetCurrentUserId();
        }

        /// <inheritdoc />
        public override Task UpdateOrAddAsync(T item, CancellationToken cancellationToken = default) {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.DateModified = TimeProvider.UtcNow;
            item.ModifiedBy = CurrentUserProvider.GetCurrentUserId();
            if (!item.CreatedBy.HasValue()) {
                item.CreatedBy = item.ModifiedBy;
                item.DateCreated = item.DateModified ?? TimeProvider.UtcNow;
            }
            return base.UpdateOrAddAsync(item, cancellationToken);
        }
        #endregion Public Methods
    }
}