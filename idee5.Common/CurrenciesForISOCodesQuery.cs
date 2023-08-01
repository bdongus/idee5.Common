using System.Collections.Immutable;

namespace idee5.Common;
/// <summary>
/// Parameters for querying the available currencies.
/// </summary>
public record CurrenciesForISOCodesQuery : IQuery<ImmutableArray<CurrencyInfo>> {
    /// <summary>
    /// List of ISO codes to search.
    /// </summary>
    public ImmutableArray<string> AllowedISOCodes { get; init; }
}
