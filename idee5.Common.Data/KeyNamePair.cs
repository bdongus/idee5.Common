namespace idee5.Common.Data;

/// <summary>
/// Translation of a key value to a dscribing string
/// </summary>
/// <param name="Key"> Identifier being described. </param>
/// <param name="Name"> Human readable name or description of the <see cref="Key"/> </param>
/// <typeparam name="TKey">Type of the key</typeparam>
public record struct KeyNamePair<TKey>(TKey Key, string Name);
