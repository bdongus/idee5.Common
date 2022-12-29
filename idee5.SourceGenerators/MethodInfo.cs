namespace idee5.SourceGenerators;
/// <summary>
/// Data about a class method.
/// </summary>
/// <param name="Name">Name of the method.</param>
/// <param name="Arguments">List of method parameters/arguments.</param>
internal sealed record MethodInfo(string Name, ArgumentInfo[] Arguments);