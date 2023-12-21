using System;

namespace idee5.Common;
/// <summary>
/// <see cref="DateTime"/> range.
/// </summary>
/// <remarks>
/// Create a new <see cref="DateTime"/> range.
/// </remarks>
/// <param name="start">Start of the range.</param>
/// <param name="end">End of the range.</param>
public class DateTimeRange(DateTime start, DateTime end) : IRange<DateTime>, IEquatable<DateTimeRange> {

    /// <inheritdoc/>
    public DateTime Start { get; } = start;

    /// <inheritdoc/>
    public DateTime End { get; } = end;

    /// <inheritdoc/>
    public bool Equals(DateTimeRange? other) => other is not null && Start.Equals(other.Start) && End.Equals(other.End);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DateTimeRange timeSlot && Equals(timeSlot);

    /// <inheritdoc/>
    public static bool operator ==(DateTimeRange left, DateTimeRange right) => (left is null && right is null) || (left?.Equals(right) == true);

    /// <inheritdoc/>
    public static bool operator !=(DateTimeRange left, DateTimeRange right) => !(left == right);

    /// <inheritdoc/>
    public override int GetHashCode() {
        var hash = new HashCodeCombiner();
        hash.AddDateTime(Start);
        hash.AddDateTime(End);
        return hash.GetCombinedHashCode().GetHashCode();
    }

    /// <inheritdoc/>
    public bool Includes(DateTime value) => (Start <= value) && (value <= End);

    /// <inheritdoc/>
    public bool Includes(IRange<DateTime> range) => (Start <= range.Start) && (range.End <= End);

    /// <inheritdoc/>
    public bool IntersectsWith(IRange<DateTime> range) => Includes(range.Start) || Includes(range.End);
}