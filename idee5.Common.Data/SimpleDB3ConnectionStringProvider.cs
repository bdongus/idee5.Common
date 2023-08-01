using System;
using System.IO;

namespace idee5.Common.Data;
/// <summary>
/// SQLite db3 connection string provider.
/// </summary>
public class SimpleDB3ConnectionStringProvider : IConnectionStringProvider {

    /// <summary>
    /// Create a SQLite db3 connection string from the given id.
    /// </summary>
    /// <param name="connectionId">The base db3 file name.</param>
    /// <returns><paramref name="connectionId"/>.db3 in the <see cref="AppDomain"/>s data directory as connection string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionId"/> is NULL.</exception>
    public string GetConnectionString(string connectionId) {
        if (connectionId == null)
            throw new ArgumentNullException(nameof(connectionId));

        string path = "";
        if (connectionId.HasValue()) {
            // ConfigurationManager is not available in netstandard, thus we can't lookup a connectionstring
            path = Path.Combine(AppDomain.CurrentDomain.GetDataDirectory(), Path.ChangeExtension(connectionId, ".db3"));
        }
        return "Data Source=" + path;
    }
}