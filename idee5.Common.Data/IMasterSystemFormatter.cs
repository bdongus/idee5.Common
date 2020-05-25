namespace idee5.Common.Data {
    /// <summary>
    /// Interface for injecting an entity's <see cref="IMasterSystemReference"/> UI representaion.
    /// </summary>
    public interface IMasterSystemFormatter
    {
        /// <summary>
        /// Format a <see cref="IMasterSystemReference"/>.
        /// </summary>
        /// <param name="masterSystemId">The </param>
        /// <returns>the <see cref="IMasterSystemReference"/>'s UI representation.</returns>
        string FormatMasterSystemId(IMasterSystemReference masterSystemId);
    }
}