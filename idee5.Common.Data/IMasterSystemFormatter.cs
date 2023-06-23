namespace idee5.Common.Data; 
/// <summary>
/// Interface for injecting an entity's <see cref="IMasterSystemReference"/> UI representation.
/// </summary>
public interface IMasterSystemFormatter {
    /// <summary>
    /// Format a <see cref="IMasterSystemReference"/>.
    /// </summary>
    /// <param name="masterSystemRef">The reference to the master system</param>
    /// <param name="maskConfig" >Key value of the mask configuration. E.g. "workplace" or "item".</param>
    /// <returns>The <see cref="IMasterSystemReference"/>'s UI representation.</returns>
    string FormatMasterSystemId(IMasterSystemReference masterSystemRef, string maskConfig);
}