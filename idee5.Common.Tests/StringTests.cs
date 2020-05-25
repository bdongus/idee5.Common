using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;

namespace idee5.Common.Tests {
    [TestClass]
    public class StringTests {
        //private static bool IsWindows10()
        //{
        //    RegistryKey reg = Registry.LocalMachine.OpenSubKey(name: @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        //    var productName = (string) reg.GetValue(name: "ProductName");

        //    return productName.StartsWith(value: "Windows 10");
        //}

        [UnitTest, TestMethod]
        public void KeepsNonSpecialCharacterString() {
            string result = "Teststring".RemoveSpecialCharacters();
            Assert.AreEqual(expected: "Teststring", actual: result);
        }

        [UnitTest, TestMethod]
        public void RemovesNonAlpha() {
            string result = "Teststring2".RemoveSpecialCharacters(pattern: "[^A-Za-z]");
            Assert.AreEqual(expected: "Teststring", actual: result);
        }

        [UnitTest, TestMethod]
        public void RemovesNonAlphaNumeric() {
            string result = "Teststring2!".RemoveSpecialCharacters(pattern: "[^A-Za-z0-9]");
            Assert.AreEqual(expected: "Teststring2", actual: result);
        }

        [UnitTest, TestMethod]
        public void RemovesNonAlphaNumericByReplace() {
            string result = "Teststring2©".ReplaceSpecialCharacters(repChar: "", pattern: "[^A-Za-z0-9]");
            Assert.AreEqual(expected: "Teststring2", actual: result);
        }

        [UnitTest, TestMethod]
        public void RemovesNonAsciiByReplace() {
            string result = "Test©string".ReplaceSpecialCharacters();
            Assert.AreEqual(expected: "Teststring", actual: result);
            Assert.AreNotEqual(result, actual: "Test©string");
        }

        [UnitTest, TestMethod]
        public void ReplaceNonAscii() {
            string result = "Test©string".ReplaceSpecialCharacters(repChar: "??");
            Assert.AreEqual(expected: "Test??string", actual: result);
            Assert.AreNotEqual(notExpected: "Test©string", actual: result);
        }

        [UnitTest, TestMethod]
        public void CanTruncateNull() {
            const string s1 = null;
            string s2 = s1.Truncate(maxLength: 42);
            Assert.AreEqual(s1, s2);
        }

        [UnitTest, TestMethod]
        public void CanTruncateSmaller() {
            const string s1 = "trullala";
            string s2 = s1.Truncate(maxLength: 42);
            Assert.AreEqual(s1, s2);
        }

        [UnitTest, TestMethod]
        public void CanTruncateSameSize() {
            const string s1 = "trullala";
            string s2 = s1.Truncate(s1.Length);
            Assert.AreEqual(s1, s2);
        }

        [UnitTest, TestMethod]
        public void CanTruncateBigger() {
            const string s1 = "trullala";
            string s2 = s1.Truncate(maxLength: 3);
            Assert.AreEqual(expected: "tru", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanExtractTagContent() {
            const string s1 = "<Tag>trullala</Tag>";
            string s2 = s1.ExtractString(beginDelim: "<tag>", endDelim: "</tag>");
            Assert.AreEqual(expected: "trullala", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanExtractCaseSensitive() {
            const string s1 = "<Tag>trullala</Tag>";
            string s2 = s1.ExtractString(beginDelim: "<tag>", endDelim: "</tag>", caseSensitive: true);
            Assert.AreEqual(expected: "", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanReplaceString() {
            const string s1 = "TruLala";
            string s2 = s1.ReplaceStringInstance(findString: "La", replaceWith: "Lu", instance: -1, caseInsensitive: false);
            Assert.AreEqual(expected: "TruLula", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanReplaceStringCaseInsensitive() {
            const string s1 = "TruLala";
            string s2 = s1.ReplaceStringInstance(findString: "La", replaceWith: "Lu", instance: -1, caseInsensitive: true);
            Assert.AreEqual(expected: "TruLuLu", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanReplaceStringCaseInsensitiveOnce() {
            const string s1 = "TruLala";
            string s2 = s1.ReplaceStringInstance(findString: "La", replaceWith: "Lu", instance: 1, caseInsensitive: true);
            Assert.AreEqual(expected: "TruLula", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanConvertToPascalCase() {
            const string s1 = "Immer eine Idee voraus.";
            string s2 = s1.ToPascalCase();
            Assert.AreEqual(expected: "ImmerEineIdeeVoraus", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanConvertToCamelCase() {
            const string s1 = "Immer eine Idee voraus.";
            string s2 = s1.ToCamelCase();
            Assert.AreEqual(expected: "immerEineIdeeVoraus", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanConvertPascalToCamelCase() {
            const string s1 = "ImmerEineIdeeVoraus.";
            string s2 = s1.PascalToCamelCase();
            Assert.AreEqual(expected: "immerEineIdeeVoraus.", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanConvertCamelToPascalCase() {
            const string s1 = "immerEineIdeeVoraus.";
            string s2 = s1.CamelToPascalCase();
            Assert.AreEqual(expected: "ImmerEineIdeeVoraus.", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanConvertToCaseWithSpecialChars() {
            const string s1 = "\", Immer+eine$(Idee)/voraus\"";
            string s2 = s1.ToCamelCase();
            Assert.AreEqual(expected: "immerEineIdeeVoraus", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanParseWrongInt() {
            const string s1 = "atari 8 bit";
            int i = s1.ParseInt(defaultValue: 7);
            Assert.AreEqual(expected: 7, actual: i);
        }

        [UnitTest, TestMethod]
        public void CanParseInt() {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(name: "de-DE"); // set it cuz the build server can have a differing culture
            const string s1 = "1.536";
            int i = s1.ParseInt(defaultValue: 7);
            Assert.AreEqual(expected: 1536, actual: i);
        }

        [UnitTest, TestMethod]
        public void CanParseWrongDec() {
            const string s1 = "CHF 1'536.42";
            decimal d = s1.ParseDecimal(defaultValue: 15.36m, numberFormat: CultureInfo.CreateSpecificCulture(name: "de-CH"));
            // Windows 10 rechnet hier anders !! Da seit 8.1 die gelieferte OS-Version immer 6.2 ist,
            // ist der Test sinnlos !!!! if (Environment.OSVersion.Version.Major == 6 &&
            // Environment.OSVersion.Version.Minor == 2) Assert.AreEqual(expected: 1536.42m, actual: d);
            //Assert.AreEqual(expected: !IsWindows10() ? 15.36m : 1536.42m, actual: d);
            // Und mit netstandard läuft es wieder richtig ...
            Assert.AreEqual(expected: 15.36m, actual: d);
        }

        [UnitTest, TestMethod]
        public void CanParseDec() {
            const string s1 = "1.536,42";
            decimal d = s1.ParseDecimal(defaultValue: 15.36m, numberFormat: CultureInfo.CreateSpecificCulture(name: "de-DE"));
            Assert.AreEqual(expected: 1536.42m, actual: d);
        }

        [UnitTest, TestMethod]
        public void CanStripNonNumber() {
            const string s1 = "CHF 1'536.42";
            string s2 = s1.StripNonNumber();
            Assert.AreEqual(expected: "153642", actual: s2);
        }

        [UnitTest, TestMethod]
        public void CanValidateMod10WithBlanks() {
            const string s1 = "4 0 1 2 8 8 8 8 8 8 8 8 1 8 8 1  ";
            Assert.AreEqual(expected: true, actual: s1.IsValidMod10());
        }

        [UnitTest, TestMethod]
        public void CanValidateMod10WithSeparators() {
            const string s1 = "4-012888(88888188)1";
            Assert.AreEqual(expected: true, actual: s1.IsValidMod10());
        }

        [UnitTest, TestMethod]
        public void CanValidateCleanMod10() {
            const string s1 = "4012888888881881";
            Assert.AreEqual(expected: true, actual: s1.IsValidMod10());
        }

        [UnitTest, TestMethod]
        public void FailsToValidateWrongMod10() {
            const string s1 = "4012888888881882";
            Assert.AreEqual(expected: false, actual: s1.IsValidMod10());
        }

        [UnitTest, TestMethod]
        public void CanAbstractShorterText() {
            const string s1 = "Arsenal London";
            Assert.AreEqual(expected: "Arsenal London", actual: s1.TextAbstract(length: 100));
        }

        [UnitTest, TestMethod]
        public void CanAbstractAfterWord() {
            const string s1 = "Arsenal London";
            Assert.AreEqual(expected: "Arsenal...", actual: s1.TextAbstract(length: 10));
        }

        [UnitTest, TestMethod]
        public void CanAbstractSingleWord() {
            const string s1 = "Arsenal";
            Assert.AreEqual(expected: "Ar...", actual: s1.TextAbstract(length: 5));
        }

        [UnitTest, TestMethod]
        public void CanConvertToEnum() {
            const string s1 = "AdjustToUniversal";
            Assert.AreEqual(expected: DateTimeStyles.AdjustToUniversal, actual: s1.ToEnum<DateTimeStyles>(DateTimeStyles.None));
        }

        [UnitTest, TestMethod]
        public void CanConvertToEnumDefault() {
            const string s1 = "AdjustToUniversl";
            Assert.AreEqual(expected: DateTimeStyles.None, actual: s1.ToEnum<DateTimeStyles>(DateTimeStyles.None));
        }

        [UnitTest, TestMethod]
        public void CanParseIntoBool() {
            // Arrange

            // Act
            bool result = "true".ParseInto<bool>();
            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue(result);
        }

        [UnitTest, TestMethod]
        public void ParseIntoThrowsFormatExceptionOnWrongString() {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<System.FormatException>(() => "treu".ParseInto<bool>());
        }
    }
}