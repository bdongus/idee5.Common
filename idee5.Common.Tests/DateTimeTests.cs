using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class DateTimeTests {
        [UnitTest, TestMethod]
        public void CanDetectWeekend() {
            var date = new DateTime(year: 1970, month: 2, day: 21); // was a saturday
            Assert.IsTrue(date.DayOfWeek.IsWeekend());
        }

        [UnitTest, TestMethod]
        public void CanAddWorkdayAndDetectWeekday() {
            var date = new DateTime(year: 1970, month: 2, day: 21); // was a saturday
            date = date.AddWorkdays(days: 1);
            Assert.IsTrue(date.DayOfWeek.IsWeekday());
        }

        [UnitTest, TestMethod]
        public void CanCalculateEndOfMonth() {
            var date = new DateTime(year: 1970, month: 2, day: 21);
            date = date.EndOfTheMonth();
            Assert.AreEqual(expected: new DateTime(year: 1970, month: 2, day: 28), actual: date);
        }

        [UnitTest, TestMethod]
        public void CanCalculateLeapYearEndOfMonth() {
            var date = new DateTime(year: 1972, month: 2, day: 21);
            date = date.EndOfTheMonth();
            Assert.AreEqual(expected: new DateTime(year: 1972, month: 2, day: 29), actual: date);
        }

        [UnitTest, TestMethod]
        public void CanConvertToCyymmdd() {
            var date = new DateTime(year: 1972, month: 2, day: 21);
            Assert.AreEqual(expected: 720221, actual: date.ToCYYMMDD());
            date = new DateTime(year: 2015, month: 10, day: 21);
            Assert.AreEqual(expected: 1151021, actual: date.ToCYYMMDD());
        }

        [UnitTest, TestMethod]
        public void CanConvertTimeToInt() {
            var date = new DateTime(year: 1972, month: 2, day: 21, hour: 8, minute: 5, second: 12);
            Assert.AreEqual(expected: 80512, actual: date.TimeAsInt());
        }

        [UnitTest, TestMethod]
        public void CanConvertFromCyymmdd() {
            var date = new DateTime(year: 1972, month: 2, day: 21);
            Assert.AreEqual(expected: date, actual: 720221.ToDateTime());
        }

        [UnitTest, TestMethod]
        public void CanConvertDateAndTimeToDateTime() {
            var date = new DateTime(year: 1972, month: 2, day: 21, hour: 8, minute: 5, second: 12);
            Assert.AreEqual(expected: date, actual: 720221.ToDateTime(time: 80512));
        }

        [TestMethod]
        public void ReturnsMinValueIfZeroInt() {
            // Arrange
            const int i = 0;

            // Act
            var date = i.ToDateTime();

            // Assert
            Assert.AreEqual(DateTime.MinValue, date);
        }

        [UnitTest, TestMethod]
        public void CanConvertDateTimeToTimeSpan() {
            var date = new DateTime(year: 1900, month: 01, day: 02, hour: 2, minute: 3, second: 4);
            var expected = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4);
            TimeSpan actual = date.ToTimeSpan();
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanConvertTimeSpanToDateTime() {
            var ts = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4);
            var expected = new DateTime(year: 1900, month: 01, day: 02, hour: 2, minute: 3, second: 4);
            DateTime actual = ts.ToDateTime();
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanCalcIso8601WorkWeek() {
            var date = new DateTime(year: 2012, month: 12, day: 31);
            const int expected = 1;
            int actual = date.Iso8601WeekOfYear();
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanRoundUp() {
            // Arrange
            var date = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 3, second: 4);
            TimeSpan roundTo = TimeSpan.FromMinutes(value: 15);
            var expected = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 15, second: 0);

            // Act
            DateTime actual = date.RoundUp(roundTo);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanRoundDown() {
            // Arrange
            var date = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 3, second: 4);
            TimeSpan roundTo = TimeSpan.FromMinutes(value: 15);
            var expected = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 0, second: 0);

            // Act
            DateTime actual = date.RoundDown(roundTo);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanRoundNearest() {
            // Arrange
            var date = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 3, second: 4);
            TimeSpan roundTo = TimeSpan.FromMinutes(value: 15);
            var expected = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 0, second: 0);

            // Act
            DateTime actual = date.RoundToNearest(roundTo);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [UnitTest, TestMethod]
        public void CanSetUtcKind() {
            // Arrange
            var date = new DateTime(year: 2012, month: 12, day: 31, hour: 2, minute: 3, second: 4);

            // Act
            DateTime utcDate = date.AsUtcKind();

            // Assert
            Assert.AreNotEqual(date.Kind, utcDate.Kind);
            Assert.AreNotEqual(date.ToUniversalTime(), utcDate);
            Assert.AreEqual(date, utcDate);
        }

        [UnitTest, TestMethod]
        public void CanIterateEveryDay() {
            // Arrange
            var start = new DateTime(year: 1970, month: 2, day: 21, hour: 2, minute: 3, second: 4);
            var end = new DateTime(year: 1970, month: 3, day: 22, hour: 2, minute: 3, second: 4);
            int counter = 0;
            // Act
            foreach (DateTime day in start.NthDay(end)) {
                counter++;
            }
            // Assert
            Assert.AreEqual(30, counter);
        }

        [UnitTest, TestMethod]
        public void CanIterate3rdDay() {
            // Arrange
            var start = new DateTime(year: 1970, month: 2, day: 21, hour: 2, minute: 3, second: 4);
            var end = new DateTime(year: 1970, month: 3, day: 22, hour: 2, minute: 3, second: 4);
            int counter = 0;
            // Act
            foreach (DateTime day in start.NthDay(end, 3)) {
                counter++;
            }
            // Assert
            Assert.AreEqual(10, counter);
        }
    }
}