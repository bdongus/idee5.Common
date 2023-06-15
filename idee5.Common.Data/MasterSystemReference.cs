namespace idee5.Common.Data {
    /// <summary>
    /// The master system reference. Useful for passing just the reference data.
    /// </summary>
    /// <param name="MasterSystem"><inheritdoc cref="IMasterSystemReference.MasterSystem" path="/summary"/></param>
    /// <param name="MasterSystemHierarchy"><inheritdoc cref="IMasterSystemReference.MasterSystemHierarchy" path="/summary"/></param>
    /// <param name="MasterSystemId"><inheritdoc cref="IMasterSystemReference.MasterSystemId" path="/summary"/></param>
    public record MasterSystemReference(string? MasterSystem, string? MasterSystemHierarchy, string? MasterSystemId) : IMasterSystemReference;
}
