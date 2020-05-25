// ================================================================================================
// Project : idee5.Web
// Author : Bernd Date
// created : 04.04.2014
// Description : Utilitiy classes for web applications
// ================================================================================================
// Change history(changed on/by: )
// 1.0.0 : 04.04.2014 BD based on https://github.com/RickStrahl/WestwindToolkit/tree/master/Westwind.Utilities/Utilities
// 1.0.1 : 05.11.2015 BD Removed the javascript de-/encode. There are standard routines.
// 1.0.2 : 09.11.2015 BD new naming convention, C# 6.0
// ================================================================================================
using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Web;

namespace idee5.Common.Net46 {
    public static class WebUtils {
        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~. Same syntax
        /// that ASP.Net internally supports but this method can be used outside of the Page framework.
        /// </summary>
        /// <param name="originalUrl">
        /// Any url including those starting with ~ for virtual base path replacement
        /// </param>
        /// <returns>relative url</returns>
        public static string ResolveUrl(string originalUrl) {
            if (originalUrl == null)
                throw new ArgumentNullException(nameof(originalUrl));

            string result = originalUrl;
            if (originalUrl.HasValue()) {
                // Fix up image path for ~ root app dir directory
                if (originalUrl.StartsWith("~", StringComparison.OrdinalIgnoreCase)) {
                    result = VirtualPathUtility.ToAbsolute(originalUrl);
                }
            }
            return result;
        }

        /// <summary>
        /// This method returns a fully qualified absolute server url which includes the protocol,
        /// server, port in addition to the server relative Url.
        /// </summary>
        /// <param name="serverUrl">Any url, either app relative (~/default.aspx) or fully qualified</param>
        /// <param name="forceHttps">if true forces the url to use https</param>
        /// <returns>Fully qualified absolute server url</returns>
        public static string ResolveServerUrl(string serverUrl, bool forceHttps = false) {
            if (serverUrl == null)
                throw new ArgumentNullException(nameof(serverUrl));
            // Is it already an absolute Url?
            if (serverUrl.IndexOf("://", StringComparison.OrdinalIgnoreCase) < 0) {
                // Start by fixing up the Url an Application relative Url
                string relPath = ResolveUrl(serverUrl);

                serverUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                serverUrl += relPath;
            }

            if (forceHttps)
                serverUrl = serverUrl.Replace(oldValue: "http://", newValue: "https://");

            return serverUrl;
        }

        /// <summary>
        /// Returns the application path as a full url with scheme
        /// </summary>
        /// <returns></returns>
        public static string GetFullApplicationPath()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath.TrimEnd(trimChars: new char[] { '/' });
        }

        /// <summary>
        /// Attempts to restart the active Web Application
        /// </summary>
        /// <remarks>
        /// Requires either full trust (HttpRuntime.UnloadAppDomain) or write access to the
        /// application folder otherwise the operation will fail and return false.
        /// </remarks>
        public static bool RestartWebApplication()
        {
            bool success = true;
            try {
                // The official way requires full trust
                HttpRuntime.UnloadAppDomain();
            }
            catch {
                success = false;
            }
            if (!success) {
                // Couldn't unload with runtime -> take it offline and back online
                string filename = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, path2: "app_offline.html");
                try {
                    File.Create(filename).Close();
                    File.Delete(filename);
                    success = true;
                }
                catch {
                }
            }
            return success;
        }

        /// <summary>
        /// Returns just the path of a full url. Strips off the filename and querystring
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlPath(string url) {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            int lnAt = url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (lnAt > 0)
                return url.Substring(startIndex: 0, length: lnAt + 1);
            return "/";
        }

        /// <summary>
        /// Translates an ASP.NET path like /myapp/subdir/page.aspx into an application relative
        /// path: subdir/page.aspx. The path returned is based of the application base and starts
        /// either with a subdirectory or page name (ie. no ~)
        /// </summary>
        /// <param name="logicalPath">A logical, server root relative path (ie. /myapp/subdir/page.aspx)</param>
        /// <returns>Lower case application relative path (ie. subdir/page.aspx)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="logicalPath"/> is <c>null</c>.</exception>
        public static string GetAppRelativePath(string logicalPath) {
            if (logicalPath == null)
                throw new ArgumentNullException(nameof(logicalPath));

            logicalPath = logicalPath.ToLower(CultureInfo.CurrentCulture);

            string basePath = HttpContext.Current?.Request.ApplicationPath.ToLower(CultureInfo.CurrentCulture) ?? "/";
            if (basePath == "/")
                return logicalPath.TrimStart(trimChars: new char[] { '/' });
            else
                return logicalPath.Replace(oldValue: basePath + "/", newValue: "");
        }

        /// <summary>
        /// Translates the current ASP.NET path into an application relative path: subdir/page.aspx.
        /// </summary>
        /// <returns>
        /// The path based of the application base and starts either with a subdirectory or page name
        /// (ie. no ~)
        /// </returns>
        public static string GetAppRelativePath() { return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.Replace(oldValue: "~/", newValue: ""); }

        private static bool _knowTrustLevel;
        private static AspNetHostingPermissionLevel _trustLevel;

        /// <summary>
        /// Get the current trust level of the hosted application
        /// </summary>
        /// <returns>The highest <see cref="AspNetHostingPermissionLevel"/></returns>
        public static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            if (_knowTrustLevel)
                return _trustLevel;

            foreach (AspNetHostingPermissionLevel level in new[] {
                AspNetHostingPermissionLevel.Unrestricted,
                AspNetHostingPermissionLevel.High,
                AspNetHostingPermissionLevel.Medium,
                AspNetHostingPermissionLevel.Low,
                AspNetHostingPermissionLevel.Minimal,
                AspNetHostingPermissionLevel.None
            }) {
                try { new AspNetHostingPermission(level).Demand(); }
                catch (SecurityException) {
                    continue;
                }

                _trustLevel = level;
                _knowTrustLevel = true;
                break;
            }
            return _trustLevel;
        }
    }
}