using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace idee5.SourceGenerators;
/// <summary>
/// Data about a class.
/// </summary>
/// <param name="Name">Name of the class.</param>
/// <param name="Methods">List of class methods.</param>
/// <param name="Locations">List of symboĺ locations</param>
/// <param name="HandlerTemplate">File name of the command handler template.</param>
/// <param name="CommandTemplate">File name of the command template.</param>
/// <param name="PropertyTemplate">File name of the property template.</param>
/// <param name="Namespace">Namespace of the class.</param>
/// <param name="Description">Optional documentation/description of the class.</param>
internal sealed record ClassInfo(string Name, ImmutableArray<MethodInfo> Methods, ImmutableArray<Location> Locations, string HandlerTemplate, string? CommandTemplate, string? PropertyTemplate, string? Namespace, string? Description);