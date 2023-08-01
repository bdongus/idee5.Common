using System;

namespace idee5.Common;
public static class TimeSpanExtensions {
    /// <summary>
    /// Rounds a <see cref="TimeSpan"/> value to the nearest time span given.
    /// </summary>
    /// <param name="spanToRound">The time span to be rounded.</param>
    /// <param name="roundTo">The time span to round to.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan RoundToNearest(this TimeSpan spanToRound, TimeSpan roundTo) {
        var ticks = (long)(Math.Round(spanToRound.Ticks / (double)roundTo.Ticks) * roundTo.Ticks);
        return new TimeSpan(ticks);
    }

    /// <summary>
    /// Multiplies the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Multiply(this TimeSpan timeSpan, int multiplier) => TimeSpan.FromTicks(timeSpan.Ticks * multiplier);

    /// <summary>
    /// Multiplies the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Multiply(this TimeSpan timeSpan, float multiplier) => TimeSpan.FromTicks((long)(timeSpan.Ticks * multiplier));

    /// <summary>
    /// Multiplies the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Multiply(this TimeSpan timeSpan, long multiplier) => TimeSpan.FromTicks(timeSpan.Ticks * multiplier);

    /// <summary>
    /// Multiplies the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Multiply(this TimeSpan timeSpan, double multiplier) => TimeSpan.FromTicks((long)(timeSpan.Ticks * multiplier));

    /// <summary>
    /// Divides the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Divide(this TimeSpan timeSpan, int divisor) => TimeSpan.FromTicks(timeSpan.Ticks / divisor);

    /// <summary>
    /// Divides the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Divide(this TimeSpan timeSpan, float divisor) => TimeSpan.FromTicks((long)(timeSpan.Ticks / divisor));

    /// <summary>
    /// Divides the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Divide(this TimeSpan timeSpan, long divisor) => TimeSpan.FromTicks(timeSpan.Ticks / divisor);

    /// <summary>
    /// Divides the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Divide(this TimeSpan timeSpan, double divisor) => TimeSpan.FromTicks((long)(timeSpan.Ticks / divisor));

    /// <summary>
    /// Convert an <see cref="int"/> value of minutes to a <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="minutes"># of minutes</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);

    /// <summary>
    /// Convert an <see cref="int"/> value of seconds to a <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="seconds"># of seconds</param>
    /// <returns>The new <see cref="TimeSpan"/>.</returns>
    public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);
}