using Microsoft.Extensions.Configuration;
using System;

namespace idee5.Common.Data;
/// <summary>
/// The default master system formatter.
/// </summary>
public class DefaultMasterSystemFormatter : IMasterSystemFormatter {
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMasterSystemFormatter"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public DefaultMasterSystemFormatter(IConfiguration configuration) {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    /// <summary>
    /// Formats the master system id using <see cref="string.Format(string, object[])"/>.
    /// </summary>
    /// <param name="masterSystemRef">The master system id.</param>
    /// <param name="maskConfig">The mask config.
    /// <example><code>
    /// "SAPP0": {
    ///     "Plant1": {
    ///         "workorder": "{0} {1}",
    ///         "item": "{0}/{1}"
    ///     }
    /// }
    /// </code>
    /// <para>The format pattern depends on the used formatter. This example is based on the .net <see cref="string.Format(string, object[])"/></para>
    /// </example>
    /// <list type="table">
    /// <listheader><term>Parameter number</term><description>Content</description></listheader>
    /// <item><term>0</term><description>Master system hierarchy</description></item>
    /// <item><term>1</term><description>Master system id</description></item>
    /// </list>
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>The <see cref="IMasterSystemReference"/>'s UI representation.</returns>
    public virtual string FormatMasterSystemId(IMasterSystemReference masterSystemRef, string maskConfig) {
        if (masterSystemRef == null) {
            throw new ArgumentNullException(nameof(masterSystemRef));
        }
        string mask = configuration[$"{masterSystemRef.MasterSystem}:{masterSystemRef.MasterSystemHierarchy}:{maskConfig}"] ?? "{0} {1}";
        return string.Format(mask, masterSystemRef.MasterSystemHierarchy, masterSystemRef.MasterSystemId);
    }
}