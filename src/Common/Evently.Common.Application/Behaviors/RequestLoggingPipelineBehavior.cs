using System.Diagnostics;
using Evently.Common.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Evently.Common.Application.Behaviors;

/// <summary>
/// Behavior MediatR dédié au logging de début/fin de traitement des requêtes.
/// </summary>
internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Construit des métadonnées stables pour les traces et les logs.
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        // Enrichit la trace OpenTelemetry courante pour la recherche distribuée.
        Activity.Current?.SetTag("request.module", moduleName);
        Activity.Current?.SetTag("request.name", requestName);

        // Ajoute "Module" dans le contexte Serilog pour toutes les lignes de ce scope.
        using (LogContext.PushProperty("Module", moduleName))
        {
            logger.LogInformation("Processing request {RequestName}", requestName);

            TResponse result = await next();

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed request {RequestName}", requestName);
            }
            else
            {
                // Ajoute l'objet d'erreur métier (et ses propriétés) au contexte de log
                // pour simplifier l'analyse des incidents.
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }

    // Convention du projet : Evently.Modules.<Module>.*
    // L'index 2 correspond au nom du module.
    private static string GetModuleName(string requestName) => requestName.Split('.')[2];
}
