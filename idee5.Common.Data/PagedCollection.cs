using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace idee5.Common.Data {
    /// <summary>
    /// Represents a read-only, observable collection of paged items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">item</see> of item returned by the result.</typeparam>
    public class PagedCollection<T> : ReadOnlyObservableCollection<T> {
        private long _totalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable{T}"/> containing the current data page of items.</param>
        protected PagedCollection(IEnumerable<T> sequence)
            : this(new ObservableCollection<T>(sequence)) {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="ObservableCollection{T}"/> containing the current data page of items.</param>
        protected PagedCollection(ObservableCollection<T> collection)
            : this(collection, collection?.Count ?? throw new ArgumentNullException(nameof(collection))) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable{T}"/> containing the current data page of items.</param>
        /// <param name="totalCount">The total number of items.</param>
        public PagedCollection(IEnumerable<T> sequence, long totalCount)
            : this(new ObservableCollection<T>(sequence), totalCount) {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="ObservableCollection{T}"/> containing the current data page of items.</param>
        /// <param name="totalCount">The total number of items.</param>
        public PagedCollection(ObservableCollection<T> collection, long totalCount)
            : base(collection) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (totalCount < collection.Count)
                throw new ArgumentOutOfRangeException(nameof(totalCount));

            _totalCount = totalCount;
        }

        /// <summary>
        /// Gets or sets the total count of all items.
        /// </summary>
        /// <value>The total count of all items.</value>
        public long TotalCount {
            get => _totalCount;
            protected set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(TotalCount));
                _totalCount = value;
            }
        }
    }
}