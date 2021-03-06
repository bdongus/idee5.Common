namespace idee5.Common.Plugins {
    public interface IPluginManager<TContract> where TContract : IPluginContract {
        IPluginLoader<TContract> PluginLoader { get; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        PluginConfiguration Configuration { get; set; }

        /// <summary>
        /// Disables the plugin.
        /// </summary>
        /// <param name="name">The name.</param>
        void DisablePlugin(string name);

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="name">The class name of the plugin.</param>
        void EnablePlugin(string name);

        /// <summary>
        /// Installs a plugin.
        /// </summary>
        /// <param name="name">The name name of the plugin.</param>
        void InstallPlugin(string name);

        /// <summary>
        /// Installs the plugins.
        /// </summary>
        void InstallPlugins();

        /// <summary>
        /// Toggles a plugin from/to enabled.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        void TogglePlugin(string name);

        /// <summary>
        /// Uninstalls the plugin.
        /// </summary>
        /// <param name="name">Th name of the plugin.</param>
        void UninstallPlugin(string name);

        /// <summary>
        /// Uninstalls the plugins.
        /// </summary>
        void UninstallPlugins();
    }
}