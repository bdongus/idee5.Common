using System;

namespace idee5.Common.Data {
    public static class AppDomainExtensions {
        /// <summary>
        /// Determine the data directory of the current <see cref="AppDomain"/>.
        /// </summary>
        /// <remarks>Use <see cref="AppDomain.SetData(string, object)"/> to set the DataDirectory entry.</remarks>
        /// <returns>Data directory path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="appDomain"/> is <c>null</c>.</exception>
        public static string GetDataDirectory(this AppDomain appDomain) {
            if (appDomain == null)
                throw new ArgumentNullException(nameof(appDomain));

            var dataDirectory = appDomain.GetData("DataDirectory") as string;
            if (!dataDirectory.HasValue()) {
                dataDirectory = appDomain.BaseDirectory;
            }
            return dataDirectory ?? "";
        }
    }
}