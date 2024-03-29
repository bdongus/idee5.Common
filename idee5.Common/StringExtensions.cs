﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace idee5.Common;
/// <summary>
/// <see cref="string"/> extension methods
/// </summary>
public static partial class StringExtensions {
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
#if NETSTANDARD2_0_OR_GREATER
            return value.Substring(startIndex: 0, length: maxLength);
#else
            return value[..maxLength];
#endif
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
#if NETSTANDARD2_0_OR_GREATER
        if (suffix == null) throw new ArgumentNullException(nameof(suffix));
#else
        ArgumentNullException.ThrowIfNull(suffix);
#endif
        int strLength = maxLength - suffix.Length;
        if (value.HasValue() && suffix.Length < value.Length && value.Length > maxLength)
#if NETSTANDARD2_0_OR_GREATER
            return value.Substring(startIndex: 0, length: strLength).TrimEnd(" ") + suffix;
#else
            return value[..strLength].TrimEnd(" ") + suffix;
#endif

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
#if NETSTANDARD2_0_OR_GREATER
        if (beginDelim == null) throw new ArgumentNullException(nameof(beginDelim));
        if (endDelim == null) throw new ArgumentNullException(nameof(endDelim));
#else
        ArgumentNullException.ThrowIfNull(beginDelim);
        ArgumentNullException.ThrowIfNull(endDelim);
#endif
        int at1, at2;

        if (string.IsNullOrEmpty(source))
            return string.Empty;

        if (caseSensitive) {
            at1 = source.IndexOf(beginDelim, StringComparison.CurrentCulture);
            if (at1 == -1)
                return string.Empty;

            if (!returnDelimiters)
                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.CurrentCulture);
            else
                at2 = source.IndexOf(endDelim, at1, StringComparison.CurrentCulture);
        } else {
            at1 = source.IndexOf(beginDelim, startIndex: 0, count: source.Length, StringComparison.CurrentCultureIgnoreCase);
            if (at1 == -1)
                return string.Empty;

            if (!returnDelimiters)
                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.CurrentCultureIgnoreCase);
            else
                at2 = source.IndexOf(endDelim, at1, StringComparison.CurrentCultureIgnoreCase);
        }

        if (allowMissingEndDelimiter && at2 == -1)
#if NETSTANDARD2_0_OR_GREATER
            return source.Substring(at1 + beginDelim.Length);
#else
            return source[(at1 + beginDelim.Length)..];
#endif

        if (at1 > -1 && at2 > 1) {
            if (!returnDelimiters)
                return source.Substring(at1 + beginDelim.Length, at2 - at1 - beginDelim.Length);
            else
                return source.Substring(at1, at2 - at1 + endDelim.Length);
        }

        return string.Empty;
    }

    /// <summary>
    /// String replace function that support
    /// </summary>
    /// <param name="origString">Original input string</param>
    /// <param name="findString">The string that is to be replaced</param>
    /// <param name="replaceWith">The replacement string</param>
    /// <param name="instance">Instance of the <paramref name="findString"/> that is to be found.
    /// If Instance = -1 all are replaced</param>
    /// <param name="caseInsensitive">Case insensitivity flag</param>
    /// <returns>updated string or original string if no matches</returns>
    public static string ReplaceStringInstance(this string origString, string findString, string replaceWith,
        int instance, bool caseInsensitive) {
        if (findString != null) {
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
#if NETSTANDARD2_0_OR_GREATER
            return origString.Substring(startIndex: 0, length: at1) + replaceWith + origString.Substring(at1 + findString.Length);
#else
            return string.Concat(origString.AsSpan(start: 0, length: at1), replaceWith, origString.AsSpan(at1 + findString.Length));
#endif
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
#if NETSTANDARD2_0_OR_GREATER
        if (origString == null) throw new ArgumentNullException(nameof(origString));
        if (findString == null) throw new ArgumentNullException(nameof(findString));
        if (replaceString == null) throw new ArgumentNullException(nameof(replaceString));
#else
        ArgumentNullException.ThrowIfNull(origString);
        ArgumentNullException.ThrowIfNull(findString);
        ArgumentNullException.ThrowIfNull(replaceString);
#endif
        int at1 = 0;
        while (true) {
            if (caseInsensitive)
                at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.CurrentCultureIgnoreCase);
            else
                at1 = origString.IndexOf(findString, at1, StringComparison.CurrentCulture);

            if (at1 == -1)
                break;

#if NETSTANDARD2_0_OR_GREATER
            origString = origString.Substring(startIndex: 0, length: at1) + replaceString + origString.Substring(at1 + findString.Length);
#else
            origString = string.Concat(origString.AsSpan(start: 0, length: at1), replaceString, origString.AsSpan(at1 + findString.Length));
#endif

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
        if (phrase == null) return string.Empty;

        return ConvertCase(phrase, firstUpper: false);
    }

    /// <summary>
    /// Takes a phrase and turns it into PascalCase text. White space, punctuation and separators
    /// are stripped.
    /// </summary>
    /// <param name="phrase">Text to convert to PascalCase</param>
    public static string ToPascalCase(this string phrase) {
        if (phrase == null) return string.Empty;

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
        int start = phrase.FindIndex(ch => char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch));
        var sb = new StringBuilder(phrase.Length - start);
        bool nextUpper = firstUpper;
#if NETSTANDARD2_0_OR_GREATER
        foreach (char ch in phrase.Substring(start)) {
#else
        foreach (char ch in phrase[start..]) {
#endif
            if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSeparator(ch) || !char.IsLetterOrDigit(ch)) {
                nextUpper = true;
                continue;
            }
            if (char.IsDigit(ch)) {
                sb.Append(ch);
                nextUpper = true;
                continue;
            }

            if (nextUpper)
                sb.Append(char.ToUpper(ch, CultureInfo.CurrentCulture));
            else
                sb.Append(char.ToLower(ch, CultureInfo.CurrentCulture));

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
#if NETSTANDARD2_0_OR_GREATER
        if (word == null) throw new ArgumentNullException(nameof(word));
        return char.ToLowerInvariant(word[0]) + word.Substring(startIndex: 1);
#else
        ArgumentNullException.ThrowIfNull(word);
        return char.ToLowerInvariant(word[0]) + word[1..];
#endif
    }

    /// <summary>
    /// Convert a camelCase word into PascalCase.
    /// </summary>
    /// <param name="word">The string.</param>
    /// <returns>The converted string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="word"/> is <c>null</c>.</exception>
    public static string CamelToPascalCase(this string word) {
#if NETSTANDARD2_0_OR_GREATER
        if (word == null) throw new ArgumentNullException(nameof(word));
        return char.ToUpperInvariant(word[0]) + word.Substring(startIndex: 1);
#else
        ArgumentNullException.ThrowIfNull(word);
        return char.ToUpperInvariant(word[0]) + word[1..];
#endif
    }

    /// <summary>
    /// Parses an string into an integer.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="defaultValue">Default value to return.</param>
    /// <param name="numberFormat">the <see cref="IFormatProvider"/> to use.</param>
    /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="int"/>.</returns>
    public static int ParseInt(this string input, int defaultValue, IFormatProvider numberFormat) {
        if (int.TryParse(input, NumberStyles.Any, numberFormat, out int val))
            return val;
        else
            return defaultValue;
    }

    /// <summary>
    /// Parses an string into an integer using the current culture.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="defaultValue">Default value to return.</param>
    /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="int"/>.</returns>
    public static int ParseInt(this string input, int defaultValue) => ParseInt(input, defaultValue, CultureInfo.CurrentCulture.NumberFormat);

    /// <summary>
    /// Parses an string into an decimal. If the value can't be parsed a default value is
    /// returned instead
    /// </summary>
    /// <param name="defaultValue">Default value to return.</param>
    /// <param name="numberFormat">the <see cref="IFormatProvider"/> to use.</param>
    /// <returns>The <paramref name="defaultValue"/> or the parsed <see cref="decimal"/>.</returns>
    public static decimal ParseDecimal(this string input, decimal defaultValue, IFormatProvider numberFormat) {
        if (decimal.TryParse(input, NumberStyles.Any, numberFormat, out decimal val))
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
#if NETSTANDARD2_0_OR_GREATER
        if (input == null) throw new ArgumentNullException(nameof(input));
#else
        ArgumentNullException.ThrowIfNull(input);
#endif

        char[] chars = input.ToCharArray();
        var sb = new StringBuilder();
        for (int i = 0; i < chars.Length; i++) {
            if (char.IsDigit(chars[i]))
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
            return string.Empty;
    }

    /// <summary>
    /// Removes special characters from a given string.
    /// </summary>
    /// <param name="value">String to be changed.</param>
    /// <param name="pattern">
    /// Regex pattern defining the range of characters to be replaced. Default are all non ASCIII.
    /// </param>
    /// <returns>Changed string.</returns>
    public static string RemoveSpecialCharacters(this string value, string pattern = _nonAscii) => ReplaceSpecialCharacters(value, repChar: string.Empty, pattern: pattern);

    /// <summary>
    /// Removes HTML/XML tags from a string.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>Changed string.</returns>
    public static string RemoveTags(this string input) {
        // Will this simple expression replace all tags???
#if NETSTANDARD2_0_OR_GREATER
        Regex tagsExpression = new Regex(pattern: "</?.+?>");
#else
        Regex tagsExpression = XMLTagRegex();
#endif
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
        if (string.IsNullOrEmpty(value))
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
        if (string.IsNullOrEmpty(value))
            return value;
#if NETSTANDARD2_0_OR_GREATER
        if (forRemoving == null) throw new ArgumentNullException(nameof(forRemoving));
        while (value.StartsWith(forRemoving, StringComparison.OrdinalIgnoreCase)) { value = value.Substring(forRemoving.Length); }
#else
        ArgumentNullException.ThrowIfNull(forRemoving);
        while (value.StartsWith(forRemoving, StringComparison.OrdinalIgnoreCase)) { value = value[forRemoving.Length..]; }
#endif
        return value;
    }

    /// <summary>
    /// Add a prefix to the <see cref="string"/> if it dosn't hav it yet.
    /// </summary>
    /// <param name="value">The current <see cref="string"/>.</param>
    /// <param name="toStartWith">The prefix to check for/add.</param>
    /// <returns>A new, prefixed string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="toStartWith"/> is <c>null</c>.</exception>
    public static string EnsureStartsWith(this string value, string toStartWith) {
#if NETSTANDARD2_0_OR_GREATER
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (toStartWith == null) throw new ArgumentNullException(nameof(toStartWith));
#else
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(toStartWith);
#endif
        if (value.StartsWith(toStartWith, StringComparison.CurrentCulture))
            return value;
        // Ensure each char is removed first from input, e.g. ~/ plus /Path will equal ~/Path not ~//Path
        return toStartWith + value.TrimStart(toStartWith.ToCharArray());
    }

    /// <summary>
    /// Check if a single <see cref="char"/> is lower case.
    /// </summary>
    /// <param name="ch"></param>
    /// <returns>True if the character is a lower case character</returns>
    public static bool IsLowerCase(this char ch) => ch == char.ToLower(ch, CultureInfo.CurrentCulture);

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
#if NETSTANDARD2_0_OR_GREATER
        string hex = convertToHex.Substring(startIndex: 0, length: hexLength).PadLeft(totalWidth: 32, paddingChar: '0');
#else
        string hex = convertToHex[..hexLength].PadLeft(totalWidth: 32, paddingChar: '0');
#endif
        return Guid.TryParse(hex, out Guid output) ? output : Guid.Empty;
    }

    /// <summary>
    /// Converts the string to its hex codes.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The hex string.</returns>
    public static string ToHex(this string value) {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        var sb = new StringBuilder(value.Length);
        foreach (char c in value) {
            sb.AppendFormat(CultureInfo.CurrentCulture, "{0:x2}", new object[] { Convert.ToUInt32(c) });
        }
        return sb.ToString();
    }

    // Uri return values should not be strings
    ///<summary>
    /// Encodes a string to a safe URL base64 string
    ///</summary>
    ///<param name="value"></param>
    ///<returns>The encoded strin.</returns>
    public static string ToUrlBase64(this string value) {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        byte[] bytes = Encoding.UTF8.GetBytes(value);
        return bytes.UrlTokenEncode();
    }

    /// <summary>
    /// Decodes a URL safe base64 string back
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The decoded string. <c>NULL</c> if decoding failed</returns>
    public static string? FromUrlBase64(this string value) {
        try {
            byte[] decodedBytes = value.UrlTokenDecode();
            return decodedBytes != null ? Encoding.UTF8.GetString(decodedBytes) : null;
        }
        catch (FormatException) { return null; }
    }

    /// <summary>
    /// Compares 2 strings with invariant culture and case ignored
    /// </summary>
    /// <param name="compare">The compare.</param>
    /// <param name="compareTo">The compare to.</param>
    public static bool InvariantEquals(this string compare, string compareTo) { return string.Equals(compare, compareTo, StringComparison.OrdinalIgnoreCase); }

    /// <summary>
    /// Determines if the string is a Guid
    /// </summary>
    /// <param name="value"></param>
    /// <param name="withHyphens">true = GUID has hyphens as delimiters.</param>
    /// <returns><c>True</c> if <paramref name="value"/> is a <see cref="Guid"/>.</returns>
    public static bool IsGuid(this string value, bool withHyphens) {
        bool isGuid = false;

        if (value.HasValue()) {
            var guidRegEx = new Regex(withHyphens ? _guidWithHyphensPattern : _guidPattern);
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
#if NETSTANDARD2_0_OR_GREATER
        if (input == null) throw new ArgumentNullException(nameof(input));
#else
        ArgumentNullException.ThrowIfNull(input);
#endif
        int length = input.Length;
        if (length < 1) { return []; }

        var remain = length % 4;
        if (remain != 0) {
            length += 4 - remain;
        }
        var inArray = new char[length];
        for (int i = 0; i < input.Length; i++) {
            char ch = input[i];
            inArray[i] = ch switch {
                '-' => '+',
                '_' => '/',
                _ => ch,
            };
        }
        // pad with '='
        for (int j = input.Length; j < inArray.Length; j++) { inArray[j] = '='; }
        return Convert.FromBase64CharArray(inArray, offset: 0, length: inArray.Length);
    }

    /// <summary>
    /// Encodes a string so that it is 'safe' for URLs, files, etc..
    /// </summary>
    /// <param name="input"></param>
    /// <returns>The encoded string.</returns>
    internal static string UrlTokenEncode(this byte[] input) {
        if (input == null || input.Length < 1) { return ""; }

        string str = Convert.ToBase64String(input);
        // if there is a trailing '=', keep it
        int index = str.IndexOf('=');
        if (index < 0) {
            index = str.Length;
        }
        char[] chArray = new char[index];
        // replace problematic URL characters
        for (int i = 0; i < index; i++) {
            char ch = str[i];
            chArray[i] = ch switch {
                '+' => '-',
                '/' => '_',
                _ => ch,
            };
        }

        return new string(chArray);
    }

    /// <summary>
    /// Strips carriage returns and line feeds from the specified text.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
    public static string StripNewLines(this string input) {
#if NETSTANDARD2_0_OR_GREATER
        if (input == null) throw new ArgumentNullException(nameof(input));
#else
        ArgumentNullException.ThrowIfNull(input);
#endif
        return input.Replace(oldValue: _carriageReturnValue, newValue: "").Replace(oldValue: Environment.NewLine, newValue: "");
    }

    /// <summary>
    /// Parses a string into an array of lines broken by \r\n or \n
    /// </summary>
    /// <param name="s">String to check for lines</param>
    /// <returns>array of strings, or null if the string passed was a null</returns>
    public static string[] GetLines(this string s)
        => s?.Replace(oldValue: "\r\n", newValue: "\n").Split(['\n']) ?? [];

    /// <summary>
    /// Returns a line count for a string
    /// </summary>
    /// <param name="s">string to count lines for</param>
    /// <returns></returns>
    public static int CountLines(this string s) => string.IsNullOrEmpty(s) ? 0 : s.Split(Environment.NewLine.ToCharArray()).Length;

    /// <summary>
    /// Converts a string into bytes for storage in any byte[] types buffer or stream format
    /// (like MemoryStream).
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding">The character encoding to use. Defaults to Unicode</param>
    /// <returns></returns>
    public static byte[] StringToBytes(this string text, Encoding? encoding = null) {
        encoding ??= Encoding.Unicode;

        return encoding.GetBytes(text ?? "");
    }

    /// <summary>
    /// Converts a byte array to a string.
    /// </summary>
    /// <param name="buffer">raw string byte data</param>
    /// <param name="encoding">Character encoding to use. Defaults to Unicode</param>
    /// <returns></returns>
    public static string BytesToString(this byte[] buffer, Encoding? encoding = null) {
        if (buffer == null)
            return "";

        encoding ??= Encoding.Unicode;

        return encoding.GetString(buffer, index: 0, count: buffer.Length);
    }

    /// <summary>
    /// Returns an abstract of the provided text by returning up to Length characters of a text
    /// string. If the text is truncated a ... is appended.
    /// </summary>
    /// <param name="text">Text to abstract</param>
    /// <param name="length">Number of characters to abstract to</param>
    public static string TextAbstract(this string text, int length) => text.Truncate(length, suffix: "...");

    /// <summary>
    /// Converts a byte array into a BinHex string
    /// </summary>
    /// <param name="data">Raw data to send</param>
    /// <returns>string or empty string if input is null</returns>
    public static string BinaryToBinHex(this byte[] data) {
        if (data == null)
            return "";
        var sb = new StringBuilder(data.Length * 2);
        foreach (byte val in data)
            sb.AppendFormat(CultureInfo.CurrentCulture, "{0:x2}", new object[] { val });
        return sb.ToString();
    }

    /// <summary>
    /// Retrieves a value from an XML-like string
    /// </summary>
    /// <param name="propertyString">The tagged string</param>
    /// <param name="tag">The tag name without brackets</param>
    public static string GetProperty(this string propertyString, string tag)
        => propertyString?.ExtractString($"<{tag}>", $"</{tag}>") ?? "";

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
        if (string.IsNullOrEmpty(numOnly)) return false;
        int sumOfDigits = 0;
        int length = numOnly.Length;
        for (int i = 0; i < length; i++) {
            var digit = numOnly[length - 1 - i] - '0';
            sumOfDigits += (i % 2 != 0) ? GetDouble(digit) : digit;
        }
        return sumOfDigits % 10 == 0;

        static int GetDouble(long i) {
            return i switch {
                0 => 0,
                1 => 2,
                2 => 4,
                3 => 6,
                4 => 8,
                5 => 1,
                6 => 3,
                7 => 5,
                8 => 7,
                9 => 9,
                _ => 0,
            };
        }
    }

    /// <summary>
    /// Determines if the given string is a valid GS1 identifier.
    /// </summary>
    /// <param name="number"></param>
    /// <returns><see langword="true"/> if the given <see cref="string"/> is a valid GS1 identifier.</returns>
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
        if (string.IsNullOrEmpty(value)) return defaultValue;

        try {
            return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
        }
        catch (ArgumentException) {
            return defaultValue;
        }
    }

    /// <summary>
    /// Shortcut for !<see cref="string.IsNullOrEmpty(string)"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><c>True</c> if <paramref name="value"/> is not <c>null</c> or emtpy.</returns>
    public static bool HasValue(this string? value) => !string.IsNullOrEmpty(value);

    /// <summary>
    /// Tries to parse a string into the supplied type by finding and using the Type's "Parse" method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns><c>Null</c> if <paramref name="value"/> could not be parsed into <typeparamref name="T"/>.</returns>
    public static T? ParseInto<T>(this string value) { return (T?)value.ParseInto(typeof(T)); }

    /// <summary>
    /// Tries to parse a string into the supplied type by finding and using the type's <see cref="TypeConverter"/>.
    /// </summary>
    /// <param name="value">Value to be parsed.</param>
    /// <param name="type">Type to parse <paramref name="value"/> into.</param>
    /// <returns>The parsed type.</returns>
    public static object? ParseInto(this string value, Type type) {
        TypeConverter tc = TypeDescriptor.GetConverter(type);
        return tc.ConvertFrom(value);
    }
#if !NETSTANDARD2_0_OR_GREATER

    [GeneratedRegex("</?.+?>")]
    private static partial Regex XMLTagRegex();
#endif
}