using idee5.Common.NET46;
using System;
using System.IO;
using System.Xml.Serialization;

namespace idee5.Common.Net46.Plugins {
    [Serializable]
    public class PluginConfiguration {
        private SerializableDictionary<string, bool> _pluginEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            PluginsPath = @".\Plugins";
            SearchRecursive = false;
            _pluginEnabled = new SerializableDictionary<string, bool>();
        }

        public static void Serialize(StreamWriter stream, PluginConfiguration c)
        {
            var xs = new XmlSerializer(typeof(PluginConfiguration));
            xs.Serialize(stream, c);
            stream.Flush();
        }

        public static PluginConfiguration Deserialize(StreamReader stream)
        {
            var xs = new XmlSerializer(typeof(PluginConfiguration));
            return (PluginConfiguration) xs.Deserialize(stream);
        }

        /// <summary>
        /// Gets or sets the location/path for the plugins.
        /// </summary>
        /// <value>The location.</value>
        public string PluginsPath { get; set; }

        /// <summary>
        /// Gets or sets the search flag.
        /// </summary>
        /// <value>True = search plugins path recursively.</value>
        public bool SearchRecursive { get; set; }

        /// <summary>
        /// Dictionary of plugins with enabled flag.
        /// </summary>
        /// <value>The plugin id and if it is enabled.</value>
        public SerializableDictionary<string, bool> PluginsEnabled {
            get { return _pluginEnabled; }
            set { _pluginEnabled = value; }
        }
    }
}