using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace idee5.Common.Data;
/// <summary>
/// The service collection extensions.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers command or query handlers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="handlerType">The handler type. E.g. typeof(IQueryHandlerAsync<,></param>
    /// <param name="serviceLifetime">The service lifetime.</param>
    /// <exception cref="ArgumentNullException">Thrown if a parameter is null.</exception>
    public static void RegisterHandlers(this IServiceCollection services, Type handlerType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (handlerType is null) throw new ArgumentNullException(nameof(handlerType));

        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.DefinedTypes.Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType && !t.Name.Contains("Validat")
            && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)));
        foreach (var item in implementations) {
            var service = new ServiceDescriptor(item.GetInterfaces().Single(i => i.GetGenericTypeDefinition() == handlerType), item, serviceLifetime);
            services.Add(service);
        }
    }
}
