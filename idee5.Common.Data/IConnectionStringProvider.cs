namespace idee5.Common.Data {
    /// <summary>
    /// Interface for connection string providers.
    /// </summary>
    public interface IConnectionStringProvider {
        /// <summary>
        /// Create a database connection string.
        /// </summary>
        /// <param name="connectionId">The id the connection string can be generated from.</param>
        /// <returns>The connection string.</returns>
        string GetConnectionString(string connectionId);
    }
}
