using System;
using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common {
    /// <summary>
    /// Extensions methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions {
        /// <summary>
        /// Base value to convert between <see cref="TimeSpan"/> and <see cref="DateTime"/>
        /// </summary>
        public static readonly DateTimeOffset TimeSpanBaseValue = new(DateTimeExtensions.TimeSpanBaseValue);

        /// <summary>
        /// Adds workdays to date. Simple version without working calendar or holidays.
        /// </summary>
        /// <param name="d">The starting date.</param>
        /// <param name="days">Workdays to be added..</param>
        /// <returns></returns>
        public static DateTimeOffset AddWorkdays(this DateTimeOffset d, int days) {
            for (int i = 0; i < days; i++) {
                d = d.AddDays(1.0);
                // leap over the weekend
                while (d.DayOfWeek.IsWeekend())
                    d = d.AddDays(1.0);
            }
            return d;
        }

        /// <summary>
        /// returns the end of month date to a given date..
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>End of month's date.</returns>
        public static DateTimeOffset EndOfTheMonth(this DateTimeOffset date) {
            return date.AddDays(-date.Day + 1).AddMonths(months: 1).AddDays(-1);
        }

        /// <summary>
        /// Iterator method for date ranges
        /// </summary>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <param name="interval">Iteration interval. Default = 1.</param>
        /// <returns></returns>
        public static IEnumerable<DateTimeOffset> NthDay(this DateTimeOffset from, DateTimeOffset to, int interval = 1) {
            for (DateTimeOffset day = from.Date; day.Date <= to.Date; day = day.AddDays(interval))
                yield return day;
        }

        #region conversions

        /// <summary>
        /// Gets the ISO 8601 week of year.
        /// </summary>
        /// <param name="instant">The time.</param>
        /// <returns>the week of the year according to ISO 8601</returns>
        public static int Iso8601WeekOfYear(this DateTimeOffset instant) {
            // explanation https://blogs.msdn.microsoft.com/shawnste/2006/01/24/iso-8601-week-of-year-format-in-microsoft-net/
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(instant.DateTime);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) {
                instant = instant.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(instant.DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Convert a DateTime to an integer representation with format CYYMMDD.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The CYYMMDD integer</returns>
        public static int ToCYYMMDD(this DateTimeOffset date) {
            int cyymmdd = date.Day;
            cyymmdd += date.Month * 100;
            cyymmdd += date.Year % 100 * 10000;
            cyymmdd += (date.Year - 1900) / 100 * 1000000;

            return cyymmdd;
        }

        /// <summary>
        /// Times as int.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The HHMMSS integer</returns>
        public static int TimeAsInt(this DateTimeOffset date) {
            int time = date.Second;
            time += date.Minute * 100;
            time += date.Hour * 10000;

            return time;
        }

        /// <summary>
        /// Convert integer date and time to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="date">CYYMMDD date.</param>
        /// <param name="time">HHMMSS time.</param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this int date, int time = 0) {
            // If there is no date, just return the minimum value. No need to waste time.
            if (date == 0)
                return DateTimeOffset.MinValue;
            int year = 1900 + (100 * (date / 1000000)) + (date % 1000000 / 10000);
            int month = date % 10000 / 100;
            int day = date % 100;

            int hour = time / 10000;
            int minute = time % 10000 / 100;
            int second = time % 100;

            return new DateTime(year, month, day, hour, minute, second);
        }

        #endregion conversions

        #region rounding

        /// <summary>
        /// Round a <see cref="DateTimeOffset"/> up.
        /// </summary>
        /// <param name="dt">This instance.</param>
        /// <param name="d"><see cref="TimeSpan"/> interval to round up to.</param>
        /// <returns>A new <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset RoundUp(this DateTimeOffset dt, TimeSpan d) {
            long modTicks = dt.Ticks % d.Ticks;
            long delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return dt.AddTicks(delta);
        }

        /// <summary>
        /// Round a <see cref="DateTimeOffset"/> down.
        /// </summary>
        /// <param name="dt">This instance.</param>
        /// <param name="d"><see cref="TimeSpan"/> interval to round down to.</param>
        /// <returns>A new <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset RoundDown(this DateTimeOffset dt, TimeSpan d) {
            long delta = dt.Ticks % d.Ticks;
            return dt.AddTicks(-delta);
        }

        /// <summary>
        /// Round a <see cref="DateTimeOffset"/> up or down. Depending on which border is nearer.
        /// </summary>
        /// <param name="dt">This instance.</param>
        /// <param name="d"><see cref="TimeSpan"/> interval to round to.</param>
        /// <returns>A new <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset RoundToNearest(this DateTimeOffset dt, TimeSpan d) {
            long delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            long offset = roundUp ? d.Ticks : 0;

            return dt.AddTicks(offset - delta);
        }

        #endregion rounding

        #region time span conversions

        /// <summary> Convert a date and time value into a time span. Baseline: 01.01.1900. <see
        /// cref="TimeSpanBaseValue"/> To be able to store time spans in databases. </summary>
        /// <param name="date">The date.</param>
        /// <returns>The DateTime value.</returns>
        public static TimeSpan ToTimeSpan(this DateTimeOffset date) {
            return date - TimeSpanBaseValue;
        }

        /// <summary>
        /// Converts a time span into a date and time.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this TimeSpan span) {
            return TimeSpanBaseValue + span;
        }

        #endregion time span conversions
    }
}