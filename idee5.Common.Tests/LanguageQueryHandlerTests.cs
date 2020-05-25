using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace idee5.Common.Tests {
    [TestClass()]
    public class LanguageQueryHandlerTests {
        //private static bool IsWindows10()
        //{
        //    RegistryKey reg = Registry.LocalMachine.OpenSubKey(name: @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        //    var productName = (string) reg.GetValue(name: "ProductName");

        //    return productName.StartsWith(value: "Windows 10");
        //}

        [UnitTest, TestMethod()]
        public void CanHandleCaseSensitive()
        {
            IDictionary<string, System.Globalization.CultureInfo> result = new LanguageQueryHandler().Handle(new LanguageQuery { LanguageFilter = "schweiz", IsCaseSensitiveQuery = true, TypeFilter = System.Globalization.CultureTypes.SpecificCultures });
            Assert.AreEqual(expected: 0, actual: result.Count);

            result = new LanguageQueryHandler().Handle(new LanguageQuery { LanguageFilter = "Schweiz", IsCaseSensitiveQuery = true, TypeFilter = System.Globalization.CultureTypes.SpecificCultures });
            Assert.AreEqual(expected: 1, actual: result.Count);
            Assert.AreEqual(expected: "de-CH", actual: result.First().Key);
        }

        [UnitTest, TestMethod]
        public void CanhandleCaseInsensitive()
        {
            IDictionary<string, System.Globalization.CultureInfo> result = new LanguageQueryHandler().Handle(new LanguageQuery { LanguageFilter = "schweiz", IsCaseSensitiveQuery = false, TypeFilter = System.Globalization.CultureTypes.SpecificCultures });
            Assert.AreEqual(expected: 1, actual: result.Count);
            Assert.AreEqual(expected: "de-CH", actual: result.First().Key);

            result = new LanguageQueryHandler().Handle(new LanguageQuery { LanguageFilter = "Schweiz", IsCaseSensitiveQuery = false, TypeFilter = System.Globalization.CultureTypes.SpecificCultures });
            Assert.AreEqual(expected: 1, actual: result.Count);
            Assert.AreEqual(expected: "de-CH", actual: result.First().Key);
        }

        [UnitTest, TestMethod]
        public void CanHandleMultipleCultureTypes()
        {
            IDictionary<string, System.Globalization.CultureInfo> result = new LanguageQueryHandler().Handle(new LanguageQuery { LanguageFilter = "deutsch", IsCaseSensitiveQuery = false, TypeFilter = System.Globalization.CultureTypes.SpecificCultures | System.Globalization.CultureTypes.NeutralCultures });
            //Assert.AreEqual(expected: IsWindows10() ? 7 : 6, actual: result.Count);
            // Oh well the german language is conquering the world. The creators update raised the count to 8.
            Assert.IsTrue(result.Count >= 6);
            Assert.AreEqual(expected: "de", actual: result.First().Key);
        }
    }
}