using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common;


/// <summary>
/// Query the available languages with filtering.
/// </summary>
/// <param name="LanguageFilter"> Text to search for in the language name. </param>
/// <param name="IsCaseSensitiveQuery"> Do a case sensitive filtering or not. </param>
/// <param name="TypeFilter"> <see cref="CultureTypes"/> to be queried. </param>
public record LanguageQuery(string LanguageFilter, bool IsCaseSensitiveQuery, CultureTypes TypeFilter) : IQuery<IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Creates a new language query.
    /// </summary>
    public LanguageQuery() : this("", false, default) {
    }
}