namespace idee5.SourceGenerators;
/// <summary>
/// Data about a method argument
/// </summary>
/// <param name="Name">Name of the argument.</param>
/// <param name="Type">Name of the argument type.</param>
/// <param name="Description">Optional documentation/description of the argument.</param>
internal sealed record ArgumentInfo(string Name, string Type, string? Description);