using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Evently.Common.Presentation.Endpoints;

/// <summary>
/// Helpers de découverte et mapping automatique des endpoints minimal API.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Scanne les assemblies et enregistre toutes les implémentations de <see cref="IEndpoint"/>.
    /// </summary>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, params Assembly[] assemblies)
    {
        ServiceDescriptor[] serviceDescriptors = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        // TryAddEnumerable évite les doublons si AddEndpoints est invoqué plusieurs fois.
        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Mappe tous les endpoints enregistrés, soit sur l'app complète, soit sur un groupe de routes.
    /// </summary>
    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        // Permet de réutiliser la même mécanique pour un group API versionné / préfixé.
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
