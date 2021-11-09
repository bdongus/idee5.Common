using System;

namespace idee5.Common {
    /// <summary>
    /// <see cref="DateTime"/> range.
    /// </summary>
    public class DateTimeRange : IRange<DateTime> {
        /// <summary>
        /// Create a new <see cref="DateTime"/> range.
        /// </summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">End of the range.</param>
        public DateTimeRange(DateTime start, DateTime end) {
            Start = start;
            End = end;
        }

        /// <inheritdoc/>
        public DateTime Start { get; }
        /// <inheritdoc/>
        public DateTime End { get; }

        /// <inheritdoc/>
        public bool Includes(DateTime value) => (Start <= value) && (value <= End);

        /// <inheritdoc/>
       public bool Includes(IRange<DateTime> range) => (Start <= range.Start) && (range.End <= End);

         /// <inheritdoc/>
       public bool IntersectsWith(IRange<DateTime> range) => Includes(range.Start) || Includes(range.End);
    }
}
