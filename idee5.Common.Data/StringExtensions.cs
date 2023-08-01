using System;
using System.IO;

namespace idee5.Common.Data {
    public static class StringExtensions {
        /// <summary>
        /// Replace the "|DataDirectory|" placeholder
        /// </summary>
        /// <param name="fileName">File name containing the placeholder.</param>
        /// <returns>The expanded file path.</returns>
        public static string ReplaceDataDirectory(this string fileName) {
            const string placeholder = "|DataDirectory|";
            var result = fileName;
            if (fileName?.Contains(placeholder) ?? false) {
                result = Path.Combine(AppDomain.CurrentDomain.GetDataDirectory(), fileName.Substring(placeholder.Length));
            }
            return result;
        }
    }
}