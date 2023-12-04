using System;
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
#if NETSTANDARD2_0_OR_GREATER
        if (query == null) throw new ArgumentNullException(nameof(query));
#else
        ArgumentNullException.ThrowIfNull(query);
#endif

        var resultset = new Dictionary<string, CultureInfo>();
#if NETSTANDARD2_0_OR_GREATER
        string text = query.IsCaseSensitiveQuery ? query.LanguageFilter : query.LanguageFilter.ToLower(CultureInfo.CurrentCulture);
        resultset = query.IsCaseSensitiveQuery ? CultureInfo.GetCultures(query.TypeFilter).Where(c => c.NativeName.Contains(text)
                || c.EnglishName.Contains(text)).ToDictionary(c => c.Name, c => c)
            : CultureInfo
                .GetCultures(query.TypeFilter)
                .Where(c => c.NativeName.ToLower(CultureInfo.CurrentCulture).Contains(text)
                    || c.EnglishName.IndexOf(query.LanguageFilter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .ToDictionary(c => c.Name, c => c);
#else
        StringComparison stringComparison = query.IsCaseSensitiveQuery ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        resultset = query.IsCaseSensitiveQuery ? CultureInfo.GetCultures(query.TypeFilter).Where(c => c.NativeName.Contains(query.LanguageFilter, stringComparison)
                || c.EnglishName.Contains(query.LanguageFilter)).ToDictionary(c => c.Name, c => c)
            : CultureInfo
                .GetCultures(query.TypeFilter)
                .Where(c => c.NativeName.Contains(query.LanguageFilter, stringComparison)
                    || c.EnglishName.Contains(query.LanguageFilter, stringComparison))
                .ToDictionary(c => c.Name, c => c);
#endif
        return resultset;
    }
}