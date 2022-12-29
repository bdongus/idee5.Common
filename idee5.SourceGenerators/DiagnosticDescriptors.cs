using idee5.SourceGenerators.Properties;
using Microsoft.CodeAnalysis;

namespace idee5.SourceGenerators;
/// <summary>
/// Diagnostic descriptors.
/// </summary>
public static class DiagnosticDescriptors {
    private static readonly LocalizableString Title0001 = new LocalizableResourceString(nameof(Resources.Title0001), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat0001 = new LocalizableResourceString(nameof(Resources.MessageFormat0001), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description0001 = new LocalizableResourceString(nameof(Resources.Description0001), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// Template not configured.
    /// </summary>
    public static DiagnosticDescriptor TemplateNotConfigured = new("I50001", Title0001, MessageFormat0001, "Usage", DiagnosticSeverity.Warning, true, Description0001);

    private static readonly LocalizableString Title0002 = new LocalizableResourceString(nameof(Resources.Title0002), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat0002 = new LocalizableResourceString(nameof(Resources.MessageFormat0002), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description0002 = new LocalizableResourceString(nameof(Resources.Description0002), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// Template not found.
    /// </summary>
    public static DiagnosticDescriptor TemplateNotFound = new("I50002", Title0002, MessageFormat0002, "Usage", DiagnosticSeverity.Warning, true, Description0002);
}
