using System;
using System.IO;
using System.Linq;

namespace idee5.Common.Net46.Plugins {
    /// <summary>
    /// Universal plugin manager based on http://stefanhenneken.wordpress.com/tag/mef/ and http://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b45f1d4e
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public class PluginManager<TContract> : IDisposable, IPluginManager<TContract> where TContract : IPluginContract {
        private readonly object _syncLock = new object();
        protected Stream configStream;

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public PluginConfiguration Configuration { get; set; }

        public IPluginLoader<TContract> PluginLoader { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mef2PluginManager"/> class.
        /// </summary>
        /// <param name="configStream">The config stream.</param>
        /// <param name="loader">The MEF2 plugin loader.</param>
        public PluginManager(Stream configStream, IPluginLoader<TContract> loader)
        {
            this.configStream = configStream;
            PluginLoader = loader;
            Configuration = PluginConfiguration.Deserialize(new StreamReader(configStream));
            // create the folders if they are missing
            if (!Directory.Exists(Configuration.PluginsPath))
                Directory.CreateDirectory(Configuration.PluginsPath);
        }

        /// <summary>
        /// Release resources. Serialize configuration.
        /// </summary>
        public void Dispose()
        {
            SaveConfiguration();
            configStream.Close();
        }

        private void SaveConfiguration()
        {
            PluginConfiguration.Serialize(new StreamWriter(configStream), Configuration);
        }

        /// <summary>
        /// Loads the PluginLoader.Plugins.
        /// </summary>
        /// <returns></returns>
        public virtual void LoadPlugins()
        {
            lock (_syncLock) {
                PluginLoader.LoadPlugins(Configuration);
            }
        }

        public void InstallPlugins()
        {
            PluginLoader.Plugins.ToList().ForEach(p => p.Install());
        }

        public void InstallPlugin(string name)
        {
            PluginLoader.Plugins.Single(p => p.GetType().Name == name).Install();
        }

        public void UninstallPlugins()
        {
            PluginLoader.Plugins.ToList().ForEach(p => p.Uninstall());
        }

        public void UninstallPlugin(string name)
        {
            PluginLoader.Plugins.Single(p => p.GetType().Name == name).Uninstall();
        }

        public void EnablePlugin(string name)
        {
            Configuration.PluginsEnabled[name] = true;
            SaveConfiguration();
        }

        public void DisablePlugin(string name)
        {
            Configuration.PluginsEnabled[name] = false;
            SaveConfiguration();
        }

        public void TogglePlugin(string name)
        {
            if (Configuration.PluginsEnabled[name])
                DisablePlugin(name);
            else
                EnablePlugin(name);
        }
    }
}