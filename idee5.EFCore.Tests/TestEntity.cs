using idee5.Common.Data;

using System.ComponentModel.DataAnnotations;

namespace idee5.EFCore.Tests;

internal class TestEntity  : IAuditedEntity, IEntity{
    public int Id { get; set; }
    
    public string? SomeString { get; set; }
    public string CreatedBy { get; set; } = "UnitTest";
    public DateTime DateCreatedUTC { get; set; }
    public DateTime? DateModifiedUTC { get; set; }
    public string? ModifiedBy { get; set; }
    [Timestamp]
    public byte[] Ts { get; set; }
}
