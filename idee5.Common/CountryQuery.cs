using System.Collections.Generic;
using System.Globalization;

namespace idee5.Common;

/// <summary>
/// Parameters for querying the available countries.
/// </summary>
/// <param name="NameFilter"> Partial country name</param>
public record CountryQuery(string NameFilter) : IQuery<IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Initializes a new instance of the <see cref="CountryQuery"/> class.
    /// </summary>
    public CountryQuery() : this("") {
    }
}