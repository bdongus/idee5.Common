using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Tests {
    [TestClass]
    public class EnumerableExtensionsTests {
        private class TestItem {

            public TestItem() {
                Children = Enumerable.Empty<TestItem>();
            }

            public IEnumerable<TestItem> Children { get; set; }
        }

        private class TestCsvItem {
            public int MyIntProperty { get; set; }
            public string MyStringProperty { get; set; }
        }

        [UnitTest, TestMethod]
        public void CanFlattenComplexList() {
            var hierarchy = new TestItem() {
                Children = new List<TestItem>() {
                new TestItem() { Children = new List<TestItem>() { new TestItem() { Children = new List<TestItem>() { new TestItem() { Children = new List<TestItem>() {
                    new TestItem(),
                    new TestItem()
                } } } } } },
                new TestItem() { Children = new List<TestItem>() {
                    new TestItem() { Children = new List<TestItem>() { new TestItem() { Children = new List<TestItem>() } } },
                    new TestItem() { Children = new List<TestItem>() { new TestItem() { Children = new List<TestItem>() } } }
                } },
            }
            };

            IEnumerable<TestItem> flattened = hierarchy.Children.FlattenList(x => x.Children);

            Assert.AreEqual(expected: 10, actual: flattened.Count());
        }

        [UnitTest, TestMethod]
        public void CanFlattenSimpleList() {
            var hierarchy = new TestItem() {
                Children = new List<TestItem>() {
                new TestItem(),
                new TestItem(),
                new TestItem()
            }
            };

            IEnumerable<TestItem> flattened = hierarchy.Children.FlattenList(x => x.Children);

            Assert.AreEqual(expected: 3, actual: flattened.Count());
        }

        [UnitTest, TestMethod]
        public void InGroupsOf_ReturnsAllElements() {
            int[] integers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            IEnumerable<int>[] groupsOfTwo = integers.InGroupsOf(groupSize: 2).ToArray();

            int[] flattened = groupsOfTwo.SelectMany(x => x).ToArray();

            Assert.AreEqual(expected: groupsOfTwo.Length, actual: 5);
            Assert.AreEqual(flattened.Length, integers.Length);
            CollectionAssert.AreEquivalent(integers, flattened);

            IEnumerable<int>[] groupsOfMassive = integers.InGroupsOf(groupSize: 100).ToArray();
            Assert.AreEqual(expected: groupsOfMassive.Length, actual: 1);
            flattened = groupsOfMassive.SelectMany(x => x).ToArray();
            Assert.AreEqual(flattened.Length, integers.Length);
            CollectionAssert.AreEquivalent(integers, flattened);
        }

        [UnitTest, TestMethod]
        public void DistinctBy_ReturnsDistinctElements_AndResetsIteratorCorrectly() {
            // Arrange
            var tuple1 = new Tuple<string, string>(item1: "fruit", item2: "apple");
            var tuple2 = new Tuple<string, string>(item1: "fruit", item2: "orange");
            var tuple3 = new Tuple<string, string>(item1: "fruit", item2: "banana");
            var tuple4 = new Tuple<string, string>(item1: "fruit", item2: "banana"); // Should be filtered out
            var list = new List<Tuple<string, string>>() {
                tuple1,
                tuple2,
                tuple3,
                tuple4
            };

            // Act
            IEnumerable<Tuple<string, string>> iteratorSource = list.Distinct(x => x.Item2);

            // Assert First check distinction
            Assert.AreEqual(expected: 3, actual: iteratorSource.Count());

            // Check for iterator block mistakes - reset to original query first
            iteratorSource = list.Distinct(x => x.Item2);
            Assert.AreEqual(iteratorSource.Count(), iteratorSource.ToList().Count);
        }

        [UnitTest, TestMethod]
        public void CanTransposeEmptyArray() {
            IEnumerable<IEnumerable<char>> result = Array.Empty<string>().Transpose();
            Assert.AreEqual(expected: 0, actual: result.Count());
        }

        [UnitTest, TestMethod]
        public void CanTransposeIEnumerable() {
            IEnumerable<int[]> x = new[] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 }, new int[] { 7, 8, 9 } };
            IEnumerable<IEnumerable<int>> y = x.Transpose();
            string xstr = x.Format(separator: "::", toString: s => s.Format(separator: ","));
            string ystr = y.Format(separator: "::", toString: s => s.Format(separator: ","));
            Assert.AreEqual(expected: "1,2,3::4,5,6::7,8,9", actual: xstr);
            Assert.AreEqual(expected: "1,4,7::2,5,8::3,6,9", actual: ystr);
        }

        [UnitTest, TestMethod]
        public void CanExecute() {
            int[] ie = new[] { 1, 2, 3 };
            int outer = 0;
            string formatted = ie.Execute(x => outer += x).Format(separator: ",");
            Assert.AreEqual(expected: 6, actual: outer);
            Assert.AreEqual(expected: "1,2,3", actual: formatted);
        }

        [UnitTest, TestMethod]
        public void CanCheckContainsAll() {
            var v1 = new string[] { "idee", "5", "St. Gallen", "Berlin" };
            var v2 = new string[] { "idee", "5" };
            Assert.IsTrue(v1.ContainsAll(v2));
            Assert.IsFalse(v2.ContainsAll(v1));
        }

        [UnitTest, TestMethod]
        public void CanCheckContainsAny() {
            var v1 = new string[] { "idee", "5", "St. Gallen", "Berlin" };
            var v2 = new string[] { "idee", "5" };
            var v3 = new string[] { "idee", "6" };
            Assert.IsTrue(v1.ContainsAny(v2));
            Assert.IsTrue(v1.ContainsAny(v3));
        }

        [UnitTest, TestMethod]
        public void CanFormat() {
            var v1 = new string[] { "idee", "5", "St. Gallen", "Berlin" };

            Assert.IsTrue(v1.Format(separator: ";") == "idee;5;St. Gallen;Berlin");
        }

        [UnitTest, TestMethod]
        public void CanFormatWithFunction() {
            var v1 = new string[] { "idee", "5", "St. Gallen", "Berlin" };

            Assert.IsTrue(v1.Format(separator: ";", toString: s => String.Format(format: "\"{0}\"", arg0: s)) == "\"idee\";\"5\";\"St. Gallen\";\"Berlin\"");
        }

        [UnitTest, TestMethod]
        public void CanExportIntsToCsv() {
            var v1 = new int[] { 21, 2, 70, 45054 };

            Assert.AreEqual(expected: "\"21\",\"2\",\"70\",\"45054\"", actual: v1.ToCsv());
        }

        [UnitTest, TestMethod]
        public void CanExportFloatsToCsv() {
            var v1 = new float[] { 21.2f, 70.45054f };
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            Assert.AreEqual(expected: $"\"21{separator}2\",\"70{separator}45054\"", actual: v1.ToCsv());
        }

        [UnitTest, TestMethod]
        public void CanExportDoublesToCsv() {
            var v1 = new double[] { 21.2d, 70.45054d };
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            Assert.AreEqual(expected: $"\"21{separator}2\",\"70{separator}45054\"", actual: v1.ToCsv());
        }

        [UnitTest, TestMethod]
        public void CantExportComplexObjectToCsv() {
            var list = new List<TestCsvItem>() {
                new TestCsvItem {MyIntProperty = 21, MyStringProperty = "Half magic" },
                new TestCsvItem {MyIntProperty = 2, MyStringProperty ="Month" },
                new TestCsvItem {MyIntProperty = 1970, MyStringProperty ="Best of breed year" }
            };
            //string expectedResult = $"\"21\",\"Half magic\"{Environment.NewLine}\"2\",\"Month\"{Environment.NewLine}\"1970\",\"Best of breed year\"{Environment.NewLine}";
            Assert.AreEqual(expected: "\"idee5.Common.Tests.EnumerableExtensionsTests+TestCsvItem\",\"idee5.Common.Tests.EnumerableExtensionsTests+TestCsvItem\",\"idee5.Common.Tests.EnumerableExtensionsTests+TestCsvItem\"", actual: list.ToCsv());
        }

        [UnitTest, TestMethod]
        public void CanJoinAsString() {
            // Arrange
            string[] itemList = { "idee5", "rocks", "at", "Wacken" };

            // Act
            var result = itemList.JoinAsString(":");

            // Assert
            Assert.AreEqual("idee5:rocks:at:Wacken", result);
        }

        [UnitTest, TestMethod]
        public void DoesFilterWhereIfTrue() {
            // Arrange
            var list = new List<TestCsvItem>() {
                new TestCsvItem {MyIntProperty = 21, MyStringProperty = "Half magic" },
                new TestCsvItem {MyIntProperty = 2, MyStringProperty ="Month" },
                new TestCsvItem {MyIntProperty = 1970, MyStringProperty ="Best of breed year" }
            };

            // Act
            IEnumerable<TestCsvItem> result = list.WhereIf(true, i => i.MyIntProperty != 1970);
            // Assert

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(21, result.First().MyIntProperty);
            Assert.AreEqual(2, result.Last().MyIntProperty);
        }

        [UnitTest, TestMethod]
        public void DoesntFilterWhereIfFalse() {
            // Arrange
            var list = new List<TestCsvItem>() {
                new TestCsvItem {MyIntProperty = 21, MyStringProperty = "Half magic" },
                new TestCsvItem {MyIntProperty = 2, MyStringProperty ="Month" },
                new TestCsvItem {MyIntProperty = 1970, MyStringProperty ="Best of breed year" }
            };

            // Act
            IEnumerable<TestCsvItem> result = list.WhereIf(false, i => i.MyIntProperty == 1970);

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [UnitTest, TestMethod]
        public void CanDetectEmptyEnumerable() {
            // Arrange
            var list = Array.Empty<int>();

            // Act
            bool result = list.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(result);
        }

        [UnitTest, TestMethod]
        public void CanDetectNullEnumerable() {
            // Arrange
            IEnumerable<int> list = null;

            // Act
            bool result = list.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(result);
        }

        [UnitTest, TestMethod]
        public void CanDetectNonEmptyEnumerable() {
            // Arrange
            int[] list = { 1, 2, 3 };

            // Act
            bool result = list.IsNullOrEmpty();

            // Assert
            Assert.IsFalse(result);
        }

        [UnitTest, TestMethod]
        public void CanChunkWithoutRemainder() {
            // Arrange
            int[] list = { 1, 2, 3 };

            // Act
            var chunked = list.Chunk(3);

            // Assert
            Assert.AreEqual(1, chunked.Count());
            Assert.AreEqual(3, chunked.First().Count());
        }

        [UnitTest, TestMethod]
        public void CanChunkWithRemainder() {
            // Arrange
            int[] list = { 1, 2, 3, 4 };

            // Act
            var chunked = list.Chunk(3);

            // Assert
            Assert.AreEqual(2, chunked.Count());
            Assert.AreEqual(3, chunked.First().Count());
            Assert.AreEqual(1, chunked.Last().Count());
        }

        [UnitTest, TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsOnChunkSizeZero() {
            // Arrange
            int[] list = { 1, 2, 3 };

            // Act
            var chunked = list.Chunk(0);

            // Assert
            // due to the "yield" in the extension method we need to do something with "chunked"
            Assert.AreEqual(0, chunked.Count());
        }

        [UnitTest, TestMethod]
        public void CanChunkLessThanChunkSize() {
            // Arrange
            int[] list = { 1, 2, 3 };

            // Act
            var chunked = list.Chunk(5);

            // Assert
            Assert.AreEqual(1, chunked.Count());
            Assert.AreEqual(3, chunked.First().Count());
        }
    }
}