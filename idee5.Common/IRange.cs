using System;

namespace idee5.Common {
    /// <summary>
    /// Basic operations for ranges.
    /// </summary>
    public interface IRange<T> where T : IComparable<T> {
        /// <summary>
        /// Gets the start.
        /// </summary>
        /// <value>The start.</value>
        T Start { get; }

        /// <summary>
        /// Gets the end.
        /// </summary>
        /// <value>The end.</value>
        T End { get; }

        /// <summary>
        /// Checks if the two ranges intersect/collide.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>True if both ranges intersect.</returns>
        bool IntersectsWith(IRange<T> range);

        /// <summary>
        /// Checks if the given value is included in Start/End.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if it is with Start/End</returns>
        bool Includes(T value);

        /// <summary>
        /// Checks if the given range is within Start/End.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>True if it is with Start/End</returns>
        bool Includes(IRange<T> range);
    }
}