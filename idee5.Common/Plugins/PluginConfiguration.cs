using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace idee5.Common.Plugins {
    public class PluginConfiguration {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            PluginsPath = @".\Plugins";
            SearchRecursive = false;
            PluginsEnabled = new SerializableDictionary<string, bool>();
        }

        /// <summary>
        /// Serialize the plugin configuration to a <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="stream">stream to use for serialization.</param>
        /// <param name="c">Plugin configuration to serialize.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        public static void Serialize(StreamWriter stream, PluginConfiguration c) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var xs = new XmlSerializer(typeof(PluginConfiguration));
            xs.Serialize(stream, c);
            stream.Flush();
        }

        public static PluginConfiguration Deserialize(StreamReader stream)
        {
            var xs = new XmlSerializer(typeof(PluginConfiguration));
            PluginConfiguration pluginConfiguration = default;
            using (var xmlReader = XmlReader.Create(stream)) {
                pluginConfiguration = (PluginConfiguration) xs.Deserialize(xmlReader);
            }
            return pluginConfiguration;
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
        public SerializableDictionary<string, bool> PluginsEnabled { get; }
    }
}