namespace idee5.Common;
/// <summary>
/// Currency related information
/// </summary>
/// <param name="IsoCode">ISO code of the currency</param>
/// <param name="EnglishName">English name of the currency</param>
/// <param name="NativeName">Native name of the currency</param>
/// <param name="Symbol">Symbol of the currency</param>
public record CurrencyInfo(string IsoCode, string EnglishName, string NativeName, string Symbol);
