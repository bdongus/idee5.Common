using System.Collections.Generic;

namespace idee5.Common.Net46.Plugins {
    /// <summary>
    /// Plugin loader interface. Use your preferred DI container to implement to loading.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    public interface IPluginLoader<TContract> where TContract : IPluginContract {
        /// <summary>
        /// Gets the loaded plugins.
        /// </summary>
        /// <value>The plugins.</value>
        IEnumerable<TContract> Plugins { get; }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        /// <param name="config"></param>
        void LoadPlugins(PluginConfiguration config);
    }
}