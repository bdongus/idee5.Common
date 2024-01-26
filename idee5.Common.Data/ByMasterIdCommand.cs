using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data;
/// <summary>
/// Base command for all commands in need of a master system reference
/// </summary>
/// <param name="MasterSystem">The master system. Can be an ERP name or tenant id.</param>
/// <param name="MasterSystemHierarchy">The hierarchy for <paramref name="MasterSystemId"/>. E.g. a sales org</param>
/// <param name="MasterSystemId">The unique master system id</param>
public abstract record ByMasterIdCommand(string? MasterSystem, string? MasterSystemHierarchy, [Required(AllowEmptyStrings = false)] string MasterSystemId) : IMasterSystemReference;
