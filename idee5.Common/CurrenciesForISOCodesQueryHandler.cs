using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace idee5.Common;
/// <summary>
/// The currency query handler.
/// </summary>
public class CurrenciesForISOCodesQueryHandler : IQueryHandler<CurrenciesForISOCodesQuery, ImmutableArray<CurrencyInfo>> {
    /// <inheritdoc/>
    public ImmutableArray<CurrencyInfo> Handle(CurrenciesForISOCodesQuery query) {
        ImmutableArray<CurrencyInfo> result = ImmutableArray<CurrencyInfo>.Empty;
        if (query?.AllowedISOCodes.IsDefaultOrEmpty == false) {
            result = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(ci => ci.Name).Distinct()
                .Select(cname => new RegionInfo(cname))
                .Where(ri => query.AllowedISOCodes.Contains(ri.ISOCurrencySymbol))
                .GroupBy(ri => ri.ISOCurrencySymbol)
                .Select(g => g.First())
                .Select(ri => new CurrencyInfo(ri.ISOCurrencySymbol, ri.CurrencyEnglishName,ri.CurrencyNativeName, ri.CurrencySymbol)).ToImmutableArray();
        }
        return result;
    }
}
