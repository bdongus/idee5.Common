using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace idee5.Common {
    /// <summary>
    /// <see cref="String"/> extension methods.
    /// </summary>
    public static class StringExtensions {
        private const string _nonAscii = @"[^\u0020-\u007F]";
        private const string _guidWithHyphensPattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
        private const string _guidPattern = @"^(\{{0,1}([0-9a-fA-F]){8}([0-9a-fA-F]){4}([0-9a-fA-F]){4}([0-9a-fA-F]){4}([0-9a-fA-F]){12}\}{0,1})$";
        private const string _carriageReturnValue = "\r";

        /// <summary>
        /// Truncates the specified string to a maximum length.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="maxLength">Maximum string length.</param>
        /// <returns>The truncated string.</returns>
        public static string Truncate(this string value, int maxLength) {
            if (value.HasValue() && value.Length > maxLength)
                return value.Substring(startIndex: 0, length: maxLength);
            return value;
        }

        /// <summary>
        /// Truncates the specified string to a maximum langth and adds a suffix.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="maxLength">Maximum string length.</param>
        /// <param name="suffix">The string to mark that there was more. E.g. "..."</param>
        /// <returns>The new, truncated and sufficed string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="suffix"/> is <c>null</c>.</exception>
        public static string Truncate(this string value, int maxLength, string suffix) {
            if (suffix == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(suffix));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            int strLength = maxLength - suffix.Length;
            if (value.HasValue() && suffix.Length < value.Length && value.Length > maxLength)
                return value.Substring(startIndex: 0, length: strLength).TrimEnd(" ") + suffix;

            return value;
        }

        /// <summary>
        /// Extracts a string from between a pair of delimiters. Only the first instance is found.
        /// </summary>
        /// <param name="source">Input String to work on</param>
        /// <param name="beginDelim">Beginning delimiter</param>
        /// <param name="endDelim">Ending delimiter</param>
        /// <param name="caseSensitive">true = case sensitive search</param>
        /// <param name="allowMissingEndDelimiter"></param>
        /// <param name="returnDelimiters">true = return value includes the delimiters</param>
        /// <returns>Extracted string or ""</returns>
        /// <exception cref="ArgumentNullException"><paramref name="beginDelim"/> or <paramref name="endDelim"/> is <c>null</c>.</exception>
        public static string ExtractString(this string source, string beginDelim, string endDelim, bool caseSensitive = false, bool allowMissingEndDelimiter = false, bool returnDelimiters = false) {
            if (beginDelim == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(beginDelim));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            if (endDelim == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(endDelim));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            int at1, at2;

            if (String.IsNullOrEmpty(source))
                return String.Empty;

            if (caseSensitive) {
                at1 = source.IndexOf(beginDelim, StringComparison.CurrentCulture);
                if (at1 == -1)
                    return String.Empty;

                if (!returnDelimiters)
                    at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.CurrentCulture);
                else
                    at2 = source.IndexOf(endDelim, at1, StringComparison.CurrentCulture);
            } else {
                at1 = source.IndexOf(beginDelim, startIndex: 0, count: source.Length, StringComparison.CurrentCultureIgnoreCase);
                if (at1 == -1)
                    return String.Empty;

                if (!returnDelimiters)
                    at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.CurrentCultureIgnoreCase);
                else
                    at2 = source.IndexOf(endDelim, at1, StringComparison.CurrentCultureIgnoreCase);
            }

            if (allowMissingEndDelimiter && at2 == -1)
                return source.Substring(at1 + beginDelim.Length);

            if (at1 > -1 && at2 > 1) {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length, at2 - at1 - beginDelim.Length);
                else
                    return source.Substring(at1, at2 - at1 + endDelim.Length);
            }

            return String.Empty;
        }

        /// <summary>
        /// String replace function that support
        /// </summary>
        /// <param name="origString">Original input string</param>
        /// <param name="findString">The string that is to be replaced</param>
        /// <param name="replaceWith">The replacement string</param>
        /// <param name="instance">
        /// Instance of the FindString that is to be found. if Instance = -1 all are replaced
        /// </param>
        /// <param name="caseInsensitive">Case insensitivity flag</param>
        /// <returns>updated string or original string if no matches</returns>
        /// <exception cref="ArgumentNullException"><paramref name="findString"/> is <c>null</c>.</exception>
        public static string ReplaceStringInstance(this string origString, string findString,
                                                   string replaceWith, int instance,
                                                   bool caseInsensitive) {
            if (origString != null && findString != null) {
                if (instance == -1)
                    return ReplaceString(origString, findString, replaceWith, caseInsensitive);

                int at1 = 0;
                for (int x = 0; x < instance; x++) {
                    if (caseInsensitive)
                        at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.CurrentCultureIgnoreCase);
                    else
                        at1 = origString.IndexOf(findString, at1, StringComparison.CurrentCulture);

                    if (at1 == -1)
                        return origString;

                    if (x < instance - 1)
                        at1 += findString.Length;
                }
                return origString.Substring(startIndex: 0, length: at1) + replaceWith + origString.Substring(at1 + findString.Length);
            }
            return origString;
        }

        /// <summary>
        /// Replaces a substring within a string with another substring with optional case
        /// sensitivity turned off.
        /// </summary>
        /// <param name="origString">String to do replacements on</param>
        /// <param name="findString">The string to find</param>
        /// <param name="replaceString">The string to replace found string wiht</param>
        /// <param name="caseInsensitive">If true case insensitive search is performed</param>
        /// <returns>updated string or original string if no matches</returns>
        /// <exception cref="ArgumentNullException"><paramref name="origString"/>, <paramref name="findString"/> or <paramref name="replaceString"/> is <c>null</c>.</exception>
        public static string ReplaceString(this string origString, string findString,
                                           string replaceString, bool caseInsensitive) {
            if (origString == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(origString));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            if (findString == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(findString));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            if (replaceString == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(replaceString));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            int at1 = 0;
            while (true) {
                if (caseInsensitive)
                    at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.CurrentCultureIgnoreCase);
                else
                    at1 = origString.IndexOf(findString, at1, StringComparison.CurrentCulture);

                if (at1 == -1)
                    break;

                origString = origString.Substring(startIndex: 0, length: at1) + replaceString + origString.Substring(at1 + findString.Length);

                at1 += replaceString.Length;
            }

            return origString;
        }

        /// <summary>
        /// Takes a phrase and turns it into camelCase text. White space, punctuation and separators
        /// are stripped.
        /// </summary>
        /// <param name="phrase">Text to convert to camelCase</param>
        public static string ToCamelCase(this string phrase) {
            if (phrase == null)
                return String.Empty;

            return ConvertCase(phrase, firstUpper: false);
        }

        /// <summary>
        /// Takes a phrase and turns it into PascalCase text. White space, punctuation and separators
        /// are stripped.
        /// </summary>
        /// <param name="phrase">Text to convert to PascalCase</param>
        public static string ToPascalCase(this string phrase) {
            if (phrase == null)
                return String.Empty;

            return ConvertCase(phrase, firstUpper: true);
        }

        /// <summary>
        /// Converts the case of a phrase.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="firstUpper">if set to <c>true</c> [first upper].</param>
        /// <returns>The converted string</returns>
        private static string ConvertCase(string phrase, bool firstUpper) {
            // ignore leading special characters
            int start = phrase.FindIndex(ch => Char.IsLetterOrDigit(ch) && !Char.IsWhiteSpace(ch));
#pragma warning disable HAA0502 // Explicit new reference type allocation
            var sb = new StringBuilder(phrase.Length - start);
#pragma warning restore HAA0502 // Explicit new reference type allocation
            bool nextUpper = firstUpper;

            foreach (char ch in phrase.Substring(start)) {
                if (Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSeparator(ch) || !Char.IsLetterOrDigit(ch)) {
                    nextUpper = true;
                    continue;
                }
                if (Char.IsDigit(ch)) {
                    sb.Append(ch);
                    nextUpper = true;
                    continue;
                }

                if (nextUpper)
                    sb.Append(Char.ToUpper(ch, CultureInfo.CurrentCulture));
                else
                    sb.Append(Char.ToLower(ch, CultureInfo.CurrentCulture));

                nextUpper = false;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert a PascalCase word into camelCase.
        /// </summary>
        /// <param name="word">The string.</param>
        /// <returns>The converted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
        public static string PascalToCamelCase(this string word) {
            if (word == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(word));
#pragma warning restore HAA0502 // Explicit new reference type allocation

            return Char.ToLowerInvariant(word[0]) + word.Substring(startIndex: 1);
        }

        /// <summary>
        /// Convert a camelCase word into PascalCase.
        /// </summary>
        /// <param name="word">The string.</param>
        /// <returns>The converted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
        public static string CamelToPascalCase(this string word) {
            if (word == null)
                throw new ArgumentNullException(nameof(word));

            return Char.ToUpperInvariant(word[0]) + word.Substring(startIndex: 1);
        }

        /// <summary>
        /// Parses an string into an integer.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="defaultValue">Default value to return.</param>
        /// <param name="numberFormat">the <see cref="IFormatProvider"/> to use.</param>
        /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="Int32"/>.</returns>
        public static int ParseInt(this string input, int defaultValue, IFormatProvider numberFormat) {
            if (Int32.TryParse(input, NumberStyles.Any, numberFormat, out int val))
                return val;
            else
                return defaultValue;
        }

        /// <summary>
        /// Parses an string into an integer using the current culture.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="defaultValue">Default value to return.</param>
        /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="Int32"/>.</returns>
        public static int ParseInt(this string input, int defaultValue) => ParseInt(input, defaultValue, CultureInfo.CurrentCulture.NumberFormat);

        /// <summary>
        /// Parses an string into an decimal. If the value can't be parsed a default value is
        /// returned instead
        /// </summary>
        /// <param name="defaultValue">Default value to return.</param>
        /// <param name="numberFormat">the <see cref="IFormatProvider"/> to use.</param>
        /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="Decimal"/>.</returns>
        public static decimal ParseDecimal(this string input, decimal defaultValue, IFormatProvider numberFormat) {
            if (Decimal.TryParse(input, NumberStyles.Any, numberFormat, out decimal val))
                return val;
            else
                return defaultValue;
        }

        /// <summary>
        /// Strips all non digit values from a string and only returns the numeric string.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>A new stripped string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public static string StripNonNumber(this string input) {
#pragma warning disable HAA0502 // Explicit new reference type allocation
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            char[] chars = input.ToCharArray();
            var sb = new StringBuilder();
#pragma warning restore HAA0502 // Explicit new reference type allocation
            for (int i = 0; i < chars.Length; i++) {
                if (Char.IsDigit(chars[i]))
                    sb.Append(chars[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Replaces special characters in a given string.
        /// </summary>
        /// <param name="value">String to be changed.</param>
        /// <param name="repChar">
        /// String replacing a special character. Default value is none, so it removes them.
        /// </param>
        /// <param name="pattern">
        /// Regex pattern defining the range of characters to be replaced. Default are all non ASCIII.
        /// </param>
        /// <returns>Changed string.</returns>
        public static string ReplaceSpecialCharacters(this string value, string repChar = "", string pattern = _nonAscii) {
            if (value != null)
                return Regex.Replace(value, pattern, repChar);
            else
                return String.Empty;
        }

        /// <summary>
        /// Removes special characters from a given string.
        /// </summary>
        /// <param name="value">String to be changed.</param>
        /// <param name="pattern">
        /// Regex pattern defining the range of characters to be replaced. Default are all non ASCIII.
        /// </param>
        /// <returns>Changed string.</returns>
        public static string RemoveSpecialCharacters(this string value, string pattern = _nonAscii) => ReplaceSpecialCharacters(value, repChar: String.Empty, pattern: pattern);

        /// <summary>
        /// Removes HTML/XML tags from a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Changed string.</returns>
        public static string RemoveTags(this string input) {
            // Will this simple expression replace all tags???
#pragma warning disable HAA0502 // Explicit new reference type allocation
            var tagsExpression = new Regex(pattern: "</?.+?>");
#pragma warning restore HAA0502 // Explicit new reference type allocation
            return tagsExpression.Replace(input, replacement: " ");
        }

        /// <summary>
        /// Trims the specified value from a string; accepts a string input whereas the in-built
        /// implementation only accepts char or char[].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="forRemoving">String to remove.</param>
        /// <returns>A new, trimmed string.</returns>
        public static string Trim(this string value, string forRemoving) {
            if (!value.HasValue())
                return value;
            return value.TrimEnd(forRemoving).TrimStart(forRemoving);
        }

        /// <summary>
        /// Trims the end of a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="forRemoving">String to remove.</param>
        /// <returns>A new, trimmed string.</returns>
        public static string TrimEnd(this string value, string forRemoving) {
            if (String.IsNullOrEmpty(value))
                return value;
            while (value.EndsWith(forRemoving, StringComparison.OrdinalIgnoreCase)) { value = value.Remove(value.LastIndexOf(forRemoving, StringComparison.OrdinalIgnoreCase)); }
            return value;
        }

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="forRemoving">String to remove.</param>
        /// <returns>A new, trimmed string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="forRemoving"/> is <c>null</c>.</exception>
        public static string TrimStart(this string value, string forRemoving) {
            if (forRemoving == null)
                throw new ArgumentNullException(nameof(forRemoving));

            if (String.IsNullOrEmpty(value))
                return value;
            while (value.StartsWith(forRemoving, StringComparison.OrdinalIgnoreCase)) { value = value.Substring(forRemoving.Length); }
            return value;
        }

        /// <summary>
        /// Add a prefix to the <see cref="String"/> if it dosn't hav it yet.
        /// </summary>
        /// <param name="value">The current <see cref="String"/>.</param>
        /// <param name="toStartWith">The prefix to check for/add.</param>
        /// <returns>A new, prefixed string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="toStartWith"/> is <c>null</c>.</exception>
        public static string EnsureStartsWith(this string value, string toStartWith) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (toStartWith == null)
                throw new ArgumentNullException(nameof(toStartWith));

            if (value.StartsWith(toStartWith, StringComparison.CurrentCulture))
                return value;
            // Ensure each char is removed first from input, e.g. ~/ plus /Path will equal ~/Path not ~//Path
            return toStartWith + value.TrimStart(toStartWith.ToCharArray());
        }

        /// <summary>
        /// Check if a single <see cref="Char"/> is lower case.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>True if the character is a lower case character</returns>
        public static bool IsLowerCase(this char ch) => ch == Char.ToLower(ch, CultureInfo.CurrentCulture);

        /// <summary>
        /// Is null or white space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The is null or white space.</returns>
        public static bool IsNullOrWhiteSpace(this string value) { return (value == null) || (value.Trim().Length == 0); }

        /// <summary>
        /// If the value is null or white space return a given default value.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string IfNullOrWhiteSpace(this string value, string defaultValue) { return value.IsNullOrWhiteSpace() ? defaultValue : value; }

        /// <summary>
        /// Encodes a string as GUID.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The <see cref="Guid"/> genreated from <paramref name="value"/>.</returns>
        public static Guid EncodeAsGuid(this string value) {
            string convertToHex = value.ToHex();
            int hexLength = convertToHex.Length < 32 ? convertToHex.Length : 32;
            string hex = convertToHex.Substring(startIndex: 0, length: hexLength).PadLeft(totalWidth: 32, paddingChar: '0');
            return Guid.TryParse(hex, out Guid output) ? output : Guid.Empty;
        }

        /// <summary>
        /// Converts the string to its hex codes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The hex string.</returns>
        public static string ToHex(this string value) {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

#pragma warning disable HAA0502 // Explicit new reference type allocation
            var sb = new StringBuilder(value.Length);
#pragma warning restore HAA0502 // Explicit new reference type allocation
            foreach (char c in value) {
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0:x2}", new object[] { Convert.ToUInt32(c) });
            }
            return sb.ToString();
        }

#pragma warning disable CA1055
        // Uri return values should not be strings
        ///<summary>
        /// Encodes a string to a safe URL base64 string
        ///</summary>
        ///<param name="value"></param>
        ///<returns>The encoded strin.</returns>
        public static string ToUrlBase64(this string value) {
#pragma warning restore CA1055 // Uri return values should not be strings
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return bytes.UrlTokenEncode();
        }

#pragma warning disable CA1055 // Uri return values should not be strings
        /// <summary>
        /// Decodes a URL safe base64 string back
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The decoded string.</returns>
        public static string FromUrlBase64(this string value) {
#pragma warning restore CA1055 // Uri return values should not be strings
            try {
                byte[] decodedBytes = value.UrlTokenDecode();
                return decodedBytes != null ? Encoding.UTF8.GetString(decodedBytes, index: 0, count: decodedBytes.Length) : null;
            }
            catch (FormatException) { return null; }
        }

        /// <summary>
        /// Compares 2 strings with invariant culture and case ignored
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <param name="compareTo">The compare to.</param>
        public static bool InvariantEquals(this string compare, string compareTo) { return String.Equals(compare, compareTo, StringComparison.OrdinalIgnoreCase); }

        /// <summary>
        /// Determines if the string is a Guid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="withHyphens">true = GUID has hyphens as delimiters.</param>
        /// <returns><c>True</c> if <paramref name="value"/> is a <see cref="Guid"/>.</returns>
        public static bool IsGuid(this string value, bool withHyphens) {
            bool isGuid = false;

            if (value.HasValue()) {
#pragma warning disable HAA0502 // Explicit new reference type allocation
                var guidRegEx = new Regex(withHyphens ? _guidWithHyphensPattern : _guidPattern);
#pragma warning restore HAA0502 // Explicit new reference type allocation
                isGuid = guidRegEx.IsMatch(value);
            }
            return isGuid;
        }

        /// <summary>
        /// Decodes a string that was encoded with UrlTokenEncode
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The decoded string as byte array.</returns>
        internal static byte[] UrlTokenDecode(this string input) {
            if (input == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(input));
#pragma warning restore HAA0502 // Explicit new reference type allocation
            int length = input.Length;
            if (length < 1) { return Array.Empty<byte>(); }
            int num2 = input[length - 1] - '0';
            if ((num2 < 0) || (num2 > 10)) { return null; }
            var inArray = new char[length - 1 + num2];
            for (int i = 0; i < (length - 1); i++) {
                char ch = input[i];
                switch (ch) {
                    case '-':
                    inArray[i] = '+';
                    break;

                    case '_':
                    inArray[i] = '/';
                    break;

                    default:
                    inArray[i] = ch;
                    break;
                }
            }
            for (int j = length - 1; j < inArray.Length; j++) { inArray[j] = '='; }
            return Convert.FromBase64CharArray(inArray, offset: 0, length: inArray.Length);
        }

        /// <summary>
        /// Encodes a string so that it is 'safe' for URLs, files, etc..
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The encoded string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        internal static string UrlTokenEncode(this byte[] input) {
            if (input == null)
#pragma warning disable HAA0502 // Explicit new reference type allocation
                throw new ArgumentNullException(nameof(input));
#pragma warning restore HAA0502 // Explicit new reference type allocation
            if (input.Length < 1) { return String.Empty; }

            string str = Convert.ToBase64String(input);
            if (str == null) { return null; }
            int index = str.Length;
            while (index > 0) {
                if (str[index - 1] != '=')
                    break;
                index--;
            }
            char[] chArray = new char[index + 1];
            chArray[index] = (char) ((0x30 + str.Length) - index);
            for (int i = 0; i < index; i++) {
                char ch = str[i];
                switch (ch) {
                    case '+':
                    chArray[i] = '-';
                    break;

                    case '/':
                    chArray[i] = '_';
                    break;

                    default:
                    chArray[i] = ch;
                    break;
                }
            }
#pragma warning disable HAA0502 // Explicit new reference type allocation
            return new string(chArray);
#pragma warning restore HAA0502 // Explicit new reference type allocation
        }

        /// <summary>
        /// Strips carriage returns and line feeds from the specified text.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public static string StripNewLines(this string input) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return input.Replace(oldValue: _carriageReturnValue, newValue: "").Replace(oldValue: Environment.NewLine, newValue: "");
        }

        /// <summary>
        /// Parses a string into an array of lines broken by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        public static string[] GetLines(this string s) => s?.Replace(oldValue: "\r\n", newValue: "\n").Split(new char[] { '\n' });

        /// <summary>
        /// Returns a line count for a string
        /// </summary>
        /// <param name="s">string to count lines for</param>
        /// <returns></returns>
        public static int CountLines(this string s) => String.IsNullOrEmpty(s) ? 0 : s.Split(Environment.NewLine.ToCharArray()).Length;

        /// <summary>
        /// Converts a string into bytes for storage in any byte[] types buffer or stream format
        /// (like MemoryStream).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding">The character encoding to use. Defaults to Unicode</param>
        /// <returns></returns>
        public static byte[] StringToBytes(this string text, Encoding encoding = null) {
            if (text == null)
                return null;

            encoding = encoding ?? Encoding.Unicode;

            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="buffer">raw string byte data</param>
        /// <param name="encoding">Character encoding to use. Defaults to Unicode</param>
        /// <returns></returns>
        public static string BytesToString(this byte[] buffer, Encoding encoding = null) {
            if (buffer == null)
                return null;

            encoding = encoding ?? Encoding.Unicode;

            return encoding.GetString(buffer, index: 0, count: buffer.Length);
        }

        /// <summary>
        /// Returns an abstract of the provided text by returning up to Length characters of a text
        /// string. If the text is truncated a ... is appended.
        /// </summary>
        /// <param name="text">Text to abstract</param>
        /// <param name="length">Number of characters to abstract to</param>
        /// <returns>string</returns>
        public static string TextAbstract(this string text, int length) => text.Truncate(length, suffix: "...");

        /// <summary>
        /// Converts a byte array into a BinHex string
        /// </summary>
        /// <param name="data">Raw data to send</param>
        /// <returns>string or null if input is null</returns>
        public static string BinaryToBinHex(this byte[] data) {
            if (data == null)
                return null;

#pragma warning disable HAA0502 // Explicit new reference type allocation
            var sb = new StringBuilder(data.Length * 2);
#pragma warning restore HAA0502 // Explicit new reference type allocation
            foreach (byte val in data)
                sb.AppendFormat(CultureInfo.CurrentCulture, "{0:x2}", new object[] { val });
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves a value from an XML-like string
        /// </summary>
        /// <param name="propertyString">The tagges string.</param>
        /// <param name="tag">The tag name.</param>
        /// <returns></returns>
        public static string GetProperty(this string propertyString, string tag) => propertyString?.ExtractString($"<{tag}>", $"</{tag}>");

        /// <summary>
        /// Determines whether the given string has a valid mod 10 check sum. Non numeric characters
        /// are stripped before calculating.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>True = is a valid mod 10 string</returns>
        public static bool IsValidMod10(this string number) {
            // Thanks to:
            // http://www.codeproject.com/Tips/515367/Validate-credit-card-number-with-Mod-algorithm
            // check whether input string is null or empty
            string numOnly = number.StripNonNumber();
            if (String.IsNullOrEmpty(numOnly)) return false;
            int sumOfDigits = 0;
            int length = numOnly.Length;
            for (int i = 0; i < length; i++) {
                var digit = numOnly[length - 1 - i] - '0';
                sumOfDigits += (i % 2 != 0) ? GetDouble(digit) : digit;
            }
            return sumOfDigits % 10 == 0;
            int GetDouble(long i) {
                switch (i) {
                    case 0: return 0;
                    case 1: return 2;
                    case 2: return 4;
                    case 3: return 6;
                    case 4: return 8;
                    case 5: return 1;
                    case 6: return 3;
                    case 7: return 5;
                    case 8: return 7;
                    case 9: return 9;
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// Determines if the given string is a valid GS1 identifier.
        /// </summary>
        /// <param name="number"></param>
        /// <returns><see langword="true"/> if the given <see cref="String"/> is a valid GS1 identifier.</returns>
        public static bool IsValidGS1Id(this string number) {
            if (number.IsNullOrEmpty())
                return true;
                int sumOfDigits = 0;
                int index = 0;
                foreach (int digit in number.Where(e => e >= '0' && e <= '9').Reverse()) {
                    int temp = (digit - 48) * ((index % 2 == 0) ? 1 : 3);
                    sumOfDigits += temp;
                    index++;
                }
                return sumOfDigits % 10 == 0;
        }

        /// <summary>
        /// Convert a string into an enum
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The string to convert.</param>
        /// <param name="defaultValue">The default value, if the string cannot be converted.</param>
        /// <returns>The converted enum value.</returns>
        public static T ToEnum<T>(this string value, T defaultValue) {
            if (String.IsNullOrEmpty(value)) return defaultValue;

            try {
                return (T) Enum.Parse(typeof(T), value, ignoreCase: true);
            }
            catch (ArgumentException) {
                return defaultValue;
            }
        }

        /// <summary>
        /// Shortcut for !<see cref="String.IsNullOrEmpty(String)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns><c>True</c> if <paramref name="value"/> is not <c>null</c> or emtpy.</returns>
        public static bool HasValue(this string value) => !String.IsNullOrEmpty(value);

        /// <summary>
        /// Tries to parse a string into the supplied type by finding and using the Type's "Parse" method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns><c>Null</c> if <paramref name="value"/> could not be parsed into <typeparamref name="T"/>.</returns>
        public static T ParseInto<T>(this string value) { return (T) value.ParseInto(typeof(T)); }

        /// <summary>
        /// Tries to parse a string into the supplied type by finding and using the type's <see cref="TypeConverter"/>.
        /// </summary>
        /// <param name="value">Value to be parsed.</param>
        /// <param name="type">Type to parse <paramref name="value"/> into.</param>
        /// <returns>The parsed type.</returns>
        public static object ParseInto(this string value, Type type) {
            TypeConverter tc = TypeDescriptor.GetConverter(type);
            return tc.ConvertFrom(value);
        }
    }
}