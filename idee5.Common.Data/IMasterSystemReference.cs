namespace idee5.Common.Data;
/// <summary>
/// Master system identification of an entity.
/// </summary>
public interface IMasterSystemReference {
    /// <summary>
    /// Id of the master system itself. Can be a SAP tenant or just the name of the ERP.
    /// </summary>
    string? MasterSystem { get; }
    /// <summary>
    /// Master system's hierarchical identification. E.g. Company and/or plant.
    /// </summary>
    string? MasterSystemHierarchy { get; }

    /// <summary>
    /// Master system's unique identification within the hierarchy.
    /// </summary>
    string? MasterSystemId { get; }
}