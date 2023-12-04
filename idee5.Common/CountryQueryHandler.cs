using System;
using System.Collections.Generic;
using System.Globalization;
#if NETSTANDARD2_0_OR_GREATER
using System.Linq;
#endif

namespace idee5.Common;
/// <summary>
/// Get all country cultures meeting the <see cref="CountryQuery"/> parameters.
/// </summary>
public class CountryQueryHandler : IQueryHandler<CountryQuery, IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>All country culture infos containing the name in the native or english name</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public IDictionary<string, CultureInfo> Handle(CountryQuery query) {
#if NETSTANDARD2_0_OR_GREATER
        if (query == null) throw new ArgumentNullException(nameof(query));
#else
        ArgumentNullException.ThrowIfNull(query);
#endif

        var resultset = new Dictionary<string, CultureInfo>();

        foreach (CultureInfo item in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
            var region = new RegionInfo(item.Name);
#if NETSTANDARD2_0_OR_GREATER
            if (!resultset.ContainsKey(region.ThreeLetterISORegionName)) {
                if (item.NativeName.Contains(query.NameFilter) || item.EnglishName.Contains(query.NameFilter))
                    resultset.Add(region.ThreeLetterISORegionName, item);
            }
#else
            if (item.NativeName.Contains(query.NameFilter) || item.EnglishName.Contains(query.NameFilter))
                resultset.TryAdd(region.ThreeLetterISORegionName, item);
#endif
        }
        return resultset;
    }
}