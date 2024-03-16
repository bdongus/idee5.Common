using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace idee5.Common.Data;
/// <summary>
/// The service collection extensions.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the query handlers.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="serviceLifetime">The service lifetime.</param>
    public static void RegisterQueryHandlers(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) {
        services.RegisterHandlers(typeof(IQueryHandlerAsync<,>), serviceLifetime);
    }
    /// <summary>
    /// Register the command handlers.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="serviceLifetime">The service lifetime.</param>
    public static void RegisterCommandHandlers(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) {
        services.RegisterHandlers(typeof(ICommandHandlerAsync<>), serviceLifetime);
    }

    /// <summary>
    /// Registers command or query handlers. Ignores decorators like validation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="handlerType">The handler type. E.g. typeof(IQueryHandlerAsync<,>)</param>
    /// <param name="serviceLifetime">The service lifetime.</param>
    /// <exception cref="ArgumentNullException">Thrown if a parameter is null.</exception>
    public static void RegisterHandlers(this IServiceCollection services, Type handlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) {
#if NETSTANDARD2_0_OR_GREATER
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (handlerType is null) throw new ArgumentNullException(nameof(handlerType));
#else
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(handlerType);
#endif
        IEnumerable<TypeInfo> implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.DefinedTypes.Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType && t.IsPublic
            && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)
            && !t.DeclaredConstructors.Any(c => c.GetParameters().Any(p => t.ImplementedInterfaces.Contains(p.ParameterType)))));
        foreach (TypeInfo? item in implementations) {
            var service = new ServiceDescriptor(item.GetInterfaces().Single(i => i.GetGenericTypeDefinition() == handlerType), item, serviceLifetime);
            services.Add(service);
        }
    }
}
