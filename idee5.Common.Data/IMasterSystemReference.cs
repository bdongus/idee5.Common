namespace idee5.Common.Data {
    /// <summary>
    /// Master system identification of an entity.
    /// </summary>
    public interface IMasterSystemReference {
        /// <summary>
        /// Master system's hierarchical identification. E.g. Company and/or plant.
        /// </summary>
        string MasterSystemHierarchy { get; }

        /// <summary>
        /// Master system's unique identification within the hierarchy.
        /// </summary>
        string MasterSystemId { get; }
    }
}