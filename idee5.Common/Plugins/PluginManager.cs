using System;
using System.IO;
using System.Linq;

namespace idee5.Common.Plugins {
    /// <summary>
    /// Universal plugin manager based on http://stefanhenneken.wordpress.com/tag/mef/ and http://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b45f1d4e
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    public class PluginManager<TContract> : IDisposable, IPluginManager<TContract> where TContract : IPluginContract {
        #region Private Fields

        private readonly object _syncLock = new();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Mef2PluginManager"/> class.
        /// </summary>
        /// <param name="configStream">The config stream.</param>
        /// <param name="loader">The MEF2 plugin loader.</param>
        public PluginManager(Stream configStream, IPluginLoader<TContract> loader) {
            ConfigStream = configStream;
            PluginLoader = loader;
            Configuration = PluginConfiguration.Deserialize(new StreamReader(configStream));
            // create the folders if they are missing
            if (!Directory.Exists(Configuration.PluginsPath))
                Directory.CreateDirectory(Configuration.PluginsPath);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public PluginConfiguration Configuration { get; set; }

        public IPluginLoader<TContract> PluginLoader { get; protected set; }
        protected Stream ConfigStream { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Disable a plugin and save the changed configuration. DOESN'T reload the plugins.
        /// </summary>
        /// <param name="name">String represenation of the plugin <see cref="Type"/>.</param>
        public void DisablePlugin(string name) {
            Configuration.PluginsEnabled[name] = false;
            SaveConfiguration();
        }

        /// <summary>
        /// Enable a plugin and save the changed configuration. DOESN'T reload the plugins.
        /// </summary>
        /// <param name="name">String represenation of the plugin <see cref="Type"/>.</param>
        public void EnablePlugin(string name) {
            Configuration.PluginsEnabled[name] = true;
            SaveConfiguration();
        }

        /// <summary>
        /// Calls the Install method of the plugin"/>
        /// </summary>
        /// <param name="name">NativeName of the plugin.</param>
        public void InstallPlugin(string name) {
            PluginLoader.Plugins.Single(p => p.GetType().Name == name).Install();
        }

        /// <summary>
        /// Installs all loaded plugins.
        /// </summary>
        public void InstallPlugins() {
            PluginLoader.Plugins.ToList().ForEach(p => p.Install());
        }

        /// <summary>
        /// Loads the PluginLoader.Plugins.
        /// </summary>
        public virtual void LoadPlugins() {
            lock (_syncLock)
                PluginLoader.LoadPlugins(Configuration);
        }

        public void TogglePlugin(string name) {
            if (Configuration.PluginsEnabled[name])
                DisablePlugin(name);
            else
                EnablePlugin(name);
        }

        public void UninstallPlugin(string name) {
            PluginLoader.Plugins.Single(p => p.GetType().Name == name).Uninstall();
        }

        public void UninstallPlugins() {
            PluginLoader.Plugins.ToList().ForEach(p => p.Uninstall());
        }

        #endregion Public Methods

        #region Private Methods

        private void SaveConfiguration() {
            using (var stream = new StreamWriter(ConfigStream))
                PluginConfiguration.Serialize(stream, Configuration);
        }

        #region IDisposable Support
        /// <summary>
        /// Release resources. Serialize configuration.
        /// </summary>
        public void Disposex() {
        }

        private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    SaveConfiguration();
                    ConfigStream.Close();
                }
                _disposedValue = true;
            }
        }

        public void Dispose() {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion Private Methods
    }
}