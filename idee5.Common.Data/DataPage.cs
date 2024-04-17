using System.Collections.Generic;

namespace idee5.Common.Data;
/// <summary>
/// The data page
/// </summary>
/// <typeparam name="T"/>
public class DataPage<T> {
    /// <summary>
    /// Paging information about this data page
    /// </summary>
    public PageInfo Paging { get; set; } = new();
    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public IList<T> Data { get; set; } = [];
}
