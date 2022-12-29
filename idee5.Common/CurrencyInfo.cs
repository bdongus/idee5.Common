namespace idee5.Common {
    /// <summary>
    /// Currency related information
    /// </summary>
    public record CurrencyInfo {
        /// <summary>
        /// ISO code of the currency.
        /// </summary>
        public string IsoCode { get; init; }
        /// <summary>
        /// English name of the currency.
        /// </summary>
        public string EnglishName { get; init; }
        /// <summary>
        /// Native name of the currency.
        /// </summary>
        public string NativeName { get; init; }
        /// <summary>
        /// Symbol of the currency.
        /// </summary>
        public string Symbol { get; init; }
    }
}
