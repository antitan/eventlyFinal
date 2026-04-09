using Evently.Common.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evently.Common.Application.Behaviors;

/// <summary>
/// Behavior MediatR qui centralise la capture des exceptions non gérées.
/// </summary>
internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            // Délègue l'exécution au handler suivant de la pipeline.
            return await next(cancellationToken);
        }
        catch (Exception exception)
        {
            // Journalise le contexte minimum utile pour la corrélation en production.
            logger.LogError(exception, "Unhandled exception for {RequestName}", typeof(TRequest).Name);

            // Normalise l'exception dans un type applicatif dédié pour éviter
            // de propager des détails d'infrastructure jusqu'à la présentation.
            throw new EventlyException(typeof(TRequest).Name, innerException: exception);
        }
    }
}
