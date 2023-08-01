using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common;
/// <summary>
/// Parameters for querying the available countries.
/// </summary>
public class CountryQuery : IQuery<IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Partial country name.
    /// </summary>
    public string NameFilter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CountryQuery"/> class.
    /// </summary>
    public CountryQuery() { NameFilter = ""; }
}