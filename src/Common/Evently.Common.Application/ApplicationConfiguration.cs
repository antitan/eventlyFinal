using System.Reflection;
using Evently.Common.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Common.Application;

/// <summary>
/// Point d'entrée de la couche Application :
/// enregistre MediatR, les validators FluentValidation et les behaviors transverses.
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Configure les services partagés de la couche Application pour tous les modules.
    /// </summary>
    /// <param name="services">Collection de services DI de l'application.</param>
    /// <param name="moduleAssemblies">Assemblies des modules à scanner.</param>
    /// <returns>La même collection de services, enrichie.</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            // Enregistre automatiquement handlers/requests/events présents dans les assemblies métiers.
            config.RegisterServicesFromAssemblies(moduleAssemblies);

            // Ordre important : chaque behavior agit comme un middleware MediatR.
            // Ici : gestion des exceptions -> logs -> validation.
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        // Enregistre tous les validateurs FluentValidation trouvés dans les modules.
        // includeInternalTypes=true permet d'utiliser des validateurs internal.
        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

        return services;
    }
}
