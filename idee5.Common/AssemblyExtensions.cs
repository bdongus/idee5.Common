using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace idee5.Common;
/// <summary>
/// The assembly extensions
/// </summary>
public static class AssemblyExtensions {
    /// <summary>
    /// Gets the non-generic and public implementations of <typeparamref name="T"/> without decorators.
    /// </summary>
    /// <typeparam name="T">The interface or base type to look for</typeparam>
    /// <param name="assembly">The assembly</param>
    /// <returns>A list of types implementing <typeparamref name="T"/> in <paramref name="assembly"/></returns>
    public static IEnumerable<Type> GetImplementationsWithoutDecorators<T>(this Assembly assembly) {
        return assembly.ExportedTypes.Where(t => !t.IsAbstract && !t.IsGenericType
            && t.GetInterfaces().Any(i => i.IsGenericType && i == typeof(T))
            // check if there is a constructor with a parameter of the interface's type (decorator pattern)
            && !t.GetConstructors().Any(c => c.GetParameters().Any(p => t.GetInterfaces().Contains(p.ParameterType))));
    }

    /// <summary>
    /// Gets the non-generic and public implementations of <paramref name="interfaceType"/> without decorators.
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <param name="interfaceType">The interface or base type to look for</param>
    /// <returns>A list of types implementing <paramref name="interfaceType"/> in <paramref name="assembly"/></returns>
    public static IEnumerable<Type> GetImplementationsWithoutDecorators(this Assembly assembly, Type interfaceType) {
        return assembly.ExportedTypes.Where(t => !t.IsAbstract && !t.IsGenericType
            && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
            // check if there is a constructor with a parameter of the interface's type (decorator pattern)
            && !t.GetConstructors().Any(c => c.GetParameters().Any(p => t.GetInterfaces().Contains(p.ParameterType))));
    }
}
