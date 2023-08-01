using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace idee5.Common.Data.Tests {
    [TestClass]
    public class DataTableExtenstionsTests {
        private class TestCsvItem {
            public int MyIntProperty { get; set; }
            public string MyStringProperty { get; set; }
        }

        [UnitTest, TestMethod]
        public void CantExportComplexObjectToCsv() {
            var list = new List<TestCsvItem>() {
                new TestCsvItem {MyIntProperty = 21, MyStringProperty = "Half magic" },
                new TestCsvItem {MyIntProperty = 2, MyStringProperty ="double quote \"" },
                new TestCsvItem {MyIntProperty = 1970, MyStringProperty ="Best of breed year" }
            };
            string expectedResult = $"\"21\",\"Half magic\"{Environment.NewLine}\"2\",\"double quote \"\"\"{Environment.NewLine}\"1970\",\"Best of breed year\"{Environment.NewLine}";
            Assert.AreEqual(expected: expectedResult, actual: list.ToDataTable().ExportToCsv());
        }

        [UnitTest, TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNullException() {
            // Arrange
            System.Data.DataTable dataTable = null;

            // Act
            dataTable.ExportToCsv();

            // Assert - Exception expected

        }

        [TestMethod]
        public void CanExportWithHeaderRow() {
            // Arrange
            var list = new List<TestCsvItem>() {
                new TestCsvItem {MyIntProperty = 21, MyStringProperty = "Half magic" },
                new TestCsvItem {MyIntProperty = 2, MyStringProperty ="double quote \"" },
                new TestCsvItem {MyIntProperty = 1970, MyStringProperty ="Best of breed year" }
            };
            string expectedResult = $"\"{nameof(TestCsvItem.MyIntProperty)}\",\"{nameof(TestCsvItem.MyStringProperty)}\"{Environment.NewLine}\"21\",\"Half magic\"{Environment.NewLine}\"2\",\"double quote \"\"\"{Environment.NewLine}\"1970\",\"Best of breed year\"{Environment.NewLine}";
            var dt = list.ToDataTable();

            // Act
            var result = dt.ExportToCsv(withHeader: true);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}