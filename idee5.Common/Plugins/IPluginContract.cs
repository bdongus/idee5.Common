namespace idee5.Common.Plugins;
/// <summary>
/// Basic plugin contract. Provides the minimum the plugin manager needs.
/// IPartImportsSatisfiedNotification can be used to init the plugin after
/// all plugins have been bound. IDisposable must be used to deactivate
/// a plugin.
/// </summary>
public interface IPluginContract {
    /// <summary>
    /// Version of the plugin
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Installs the plugin
    /// </summary>
    void Install();

    /// <summary>
    /// Uninstalls the plugin
    /// </summary>
    void Uninstall();
}