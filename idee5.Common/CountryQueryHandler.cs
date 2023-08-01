using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public IDictionary<string, CultureInfo> Handle(CountryQuery query) {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        var resultset = new Dictionary<string, CultureInfo>();

        foreach (CultureInfo item in CultureInfo.GetCultures(CultureTypes.SpecificCultures)) {
            var region = new RegionInfo(item.LCID);
            if (!resultset.Keys.Contains(region.ThreeLetterISORegionName)) {
                if (item.NativeName.Contains(query.NameFilter) || item.EnglishName.Contains(query.NameFilter))
                    resultset.Add(region.ThreeLetterISORegionName, item);
            }
        }
        return resultset;
    }
}