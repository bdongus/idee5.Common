using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class RangeTests {
        internal class DateRange : IRange<DateTime> {
            public DateRange(DateTime start, DateTime end)
            {
                Start = start;
                End = end;
            }

            public DateTime Start { get; }
            public DateTime End { get; }

            public bool Includes(DateTime value)
            {
                return (Start <= value) && (value <= End);
            }

            public bool Includes(IRange<DateTime> range)
            {
                return (Start <= range.Start) && (range.End <= End);
            }

            public bool IntersectsWith(IRange<DateTime> range)
            {
                return Includes(range.Start) || Includes(range.End);
            }
        }

        [UnitTest, TestMethod]
        public void CanDetectDateInRange()
        {
            var start = new DateTime(year: 1970, month: 2, day: 21);
            DateTime end = DateTime.Now;
            var date = new DateTime(year: 2000, month: 2, day: 21);
            var range = new DateRange(start, end);
            Assert.IsTrue(range.Includes(date));
        }

        [UnitTest, TestMethod]
        public void CanDetectDateOutOfRange()
        {
            var start = new DateTime(year: 1970, month: 2, day: 21);
            var end = new DateTime(year: 2000, month: 2, day: 21);
            DateTime date = DateTime.Now;
            var range = new DateRange(start, end);
            Assert.IsFalse(range.Includes(date));
        }

        [UnitTest, TestMethod]
        public void CanDetectDateRangeInRange()
        {
            var start = new DateTime(year: 1970, month: 2, day: 21);
            DateTime end = DateTime.Now;
            var allowedRange = new DateRange(start, end);
            var requestedRange = new DateRange(start, end);
            Assert.IsTrue(allowedRange.Includes(requestedRange));
        }

        [UnitTest, TestMethod]
        public void CanDetectDateRangeOutOfRange()
        {
            var start = new DateTime(year: 1970, month: 2, day: 21);
            var end = new DateTime(year: 2000, month: 2, day: 21);
            var allowedRange = new DateRange(start, end);
            var requestedRange = new DateRange(start - TimeSpan.FromMinutes(value: 1), end);
            Assert.IsFalse(allowedRange.Includes(requestedRange));
        }
    }
}