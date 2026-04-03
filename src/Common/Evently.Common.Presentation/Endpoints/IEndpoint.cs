using Microsoft.AspNetCore.Routing;

namespace Evently.Common.Presentation.Endpoints;

/// <summary>
/// Contrat minimal pour déclarer un endpoint HTTP d'un module.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Enregistre les routes de l'endpoint dans le route builder fourni.
    /// </summary>
    void MapEndpoint(IEndpointRouteBuilder app);
}
