using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Common.Tests {
    public class QueryParameters : IQuery<string[]>
    {
        public string Searchtext { get; set; }
    }

    public class QueryHandler : IQueryHandler<QueryParameters, string[]>
    {
        /// <summary>
        /// Handles the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public string[] Handle(QueryParameters query)
        {
            return query.Searchtext.Split(separator: new char[] { ' ' });
        }
    }

    [TestClass]
    public class IQueryHandlerTests
    {
        //private static bool IsWindows10()
        //{
        //    RegistryKey reg = Registry.LocalMachine.OpenSubKey(name: @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        //    var productName = (string) reg.GetValue(name: "ProductName");

        //    return productName.StartsWith(value: "Windows 10");
        //}

        [UnitTest, TestMethod]
        public void TestQueryHandler()
        {
            var q = new QueryParameters { Searchtext = "bli bla blubb" };
            var handler = new QueryHandler();
            string[] s = handler.Handle(q);

            Assert.AreEqual(expected: 3, actual: s.Length);

            Assert.AreEqual(expected: "bla", actual: s[1]);
        }

        [UnitTest, TestMethod]
        public void CanFilterNativeCountryQuery()
        {
            var q = new CountryQuery { NameFilter = "Suisse" };
            var handler = new CountryQueryHandler();
            System.Collections.Generic.IDictionary<string, System.Globalization.CultureInfo> result = handler.Handle(q);
            Assert.AreEqual(expected: 1, actual: result.Count);
        }

        [UnitTest, TestMethod]
        public void CanFilterCountryQuery()
        {
            var q = new CountryQuery { NameFilter = "Swi" };
            var handler = new CountryQueryHandler();
            System.Collections.Generic.IDictionary<string, System.Globalization.CultureInfo> result = handler.Handle(q);
            // Djbouti and Switzerland under Windows 10 ???? Da seit 8.1 die gelieferte OS-Version
            // immer 6.2 ist, ist der Test sinnlos !!!! if (Environment.OSVersion.Version.Major==6 &&
            // Environment.OSVersion.Version.Minor==2) Assert.AreEqual(expected: 2, actual: result.Count);
            //Assert.AreEqual(expected: IsWindows10() ? 2 : 1, actual: result.Count);
            Assert.IsTrue(result.Count >= 1);
        }

        [UnitTest, TestMethod]
        public void CanGetAllCountryQuery()
        {
            var q = new CountryQuery();
            var handler = new CountryQueryHandler();
            System.Collections.Generic.IDictionary<string, System.Globalization.CultureInfo> result = handler.Handle(q);
            Assert.AreNotEqual(notExpected: 1, actual: result.Count);
        }
    }
}