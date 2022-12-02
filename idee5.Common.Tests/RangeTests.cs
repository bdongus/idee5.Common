using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Tests {

	[TestClass]
	public class RangeTests {

		[UnitTest, TestMethod]
		public void CanDetectDateInRange() {
			var start = new DateTime(year: 1970, month: 2, day: 21);
			DateTime end = DateTime.Now;
			var date = new DateTime(year: 2000, month: 2, day: 21);
			var range = new DateTimeRange(start, end);
			Assert.IsTrue(range.Includes(date));
		}

		[UnitTest, TestMethod]
		public void CanDetectDateOutOfRange() {
			var start = new DateTime(year: 1970, month: 2, day: 21);
			var end = new DateTime(year: 2000, month: 2, day: 21);
			DateTime date = DateTime.Now;
			var range = new DateTimeRange(start, end);
			Assert.IsFalse(range.Includes(date));
		}

		[UnitTest, TestMethod]
		public void CanDetectDateRangeInRange() {
			var start = new DateTime(year: 1970, month: 2, day: 21);
			DateTime end = DateTime.Now;
			var allowedRange = new DateTimeRange(start, end);
			var requestedRange = new DateTimeRange(start, end);
			Assert.IsTrue(allowedRange.Includes(requestedRange));
		}

		[UnitTest, TestMethod]
		public void CanDetectDateRangeOutOfRange() {
			var start = new DateTime(year: 1970, month: 2, day: 21);
			var end = new DateTime(year: 2000, month: 2, day: 21);
			var allowedRange = new DateTimeRange(start, end);
			var requestedRange = new DateTimeRange(start - TimeSpan.FromMinutes(value: 1), end);
			Assert.IsFalse(allowedRange.Includes(requestedRange));
		}

		[UnitTest, TestMethod]
		public void CanCompareDateRanges() {
			// Arrange
			var start = new DateTime(year: 1970, month: 2, day: 21);
			var end = new DateTime(year: 2000, month: 2, day: 21);
			// Act
			var dr1 = new DateTimeRange(start, end);
			var dr2 = new DateTimeRange(start, end);

			// Assert
			Assert.AreEqual(dr1, dr2);
		}
		[IntegrationTest, TestMethod]
		public void CanCompareDateRangesInLinq() {
			// Arrange
			var start = new DateTime(year: 1970, month: 2, day: 21);
			var end = new DateTime(year: 2000, month: 2, day: 21);
			var dr1 = new DateTimeRange(start, end);
			List<DateTimeRange> ranges = new List<DateTimeRange> {
				new DateTimeRange(start, end),
				new DateTimeRange(start, end.AddDays(5))
			};
			// Act
			var result = ranges.SingleOrDefault(dr => dr == dr1);

			// Assert
			Assert.IsNotNull(result);
		}
	}
}