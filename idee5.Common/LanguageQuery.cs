using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common;
/// <summary>
/// Query the available languages with filtering.
/// </summary>
public class LanguageQuery : IQuery<IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Text to search for in the language name.
    /// </summary>
    public string LanguageFilter { get; set; }

    /// <summary>
    /// Creates a new language query.
    /// </summary>
    public LanguageQuery() {
        LanguageFilter = "";
        IsCaseSensitiveQuery = false;
    }

    /// <summary>
    /// Do a case sensitive filtering or not.
    /// </summary>
    public bool IsCaseSensitiveQuery { get; set; }

    /// <summary>
    /// <see cref="CultureTypes"/> to be queried.
    /// </summary>
    public CultureTypes TypeFilter { get; set; }
}