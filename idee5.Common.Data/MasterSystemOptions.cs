namespace idee5.Common.Data;
/// <summary>
/// The master system configuration options
/// </summary>
public class MasterSystemOptions {
    /// <summary>
    /// Recommended section for these options 
    /// </summary>
    public const string Section = "MasterSystem";
    /// <summary>
    /// Id of the master system itself. Can be a SAP tenant or just the name of the ERP.
    /// </summary>
    public string? SystemName { get; set; }
    /// <summary>
    /// Master system's hierarchical identification. E.g. Company and/or plant.
    /// </summary>
    public string? Hierarchy { get; set; }
}
