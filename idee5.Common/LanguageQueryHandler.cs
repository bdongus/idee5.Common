using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace idee5.Common;
/// <summary>
/// <see cref="LanguageQuery"/> handler.
/// </summary>
public class LanguageQueryHandler : IQueryHandler<LanguageQuery, IDictionary<string, CultureInfo>> {
    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>
    /// All language culture infos containing the name in the native or english name
    /// </returns>
    public IDictionary<string, CultureInfo> Handle(LanguageQuery query) {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        var resultset = new Dictionary<string, CultureInfo>();
        string text = query.IsCaseSensitiveQuery ? query.LanguageFilter : query.LanguageFilter.ToLower(CultureInfo.CurrentCulture);
        resultset = query.IsCaseSensitiveQuery ? CultureInfo.GetCultures(query.TypeFilter).Where(c => c.NativeName.Contains(text)
                || c.EnglishName.Contains(text)).ToDictionary(c => c.Name, c => c)
            : CultureInfo
                .GetCultures(query.TypeFilter)
                .Where(c => c.NativeName.ToLower(CultureInfo.CurrentCulture).Contains(text)
                    || c.EnglishName.IndexOf(query.LanguageFilter, System.StringComparison.CurrentCultureIgnoreCase) >= 0)
                .ToDictionary(c => c.Name, c => c);
        return resultset;
    }
}