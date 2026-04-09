using System.Reflection;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Evently.Common.Application.Behaviors;

/// <summary>
/// Behavior MediatR chargé d'appliquer FluentValidation avant l'exécution des handlers.
/// </summary>
internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Exécute tous les validateurs associés à la requête.
        ValidationFailure[] validationFailures = await ValidateAsync(request);

        if (validationFailures.Length == 0)
        {
            // Pas d'erreur : on continue la pipeline normalement.
            return await next(cancellationToken);
        }

        // Si le handler retourne Result<T>, on fabrique dynamiquement
        // un échec de validation typé sans lever d'exception.
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type resultType = typeof(TResponse).GetGenericArguments()[0];

            MethodInfo? failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.ValidationFailure));

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, [CreateValidationError(validationFailures)]);
            }
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            // Variante non générique : renvoie un Result d'échec.
            return (TResponse)(object)Result.Failure(CreateValidationError(validationFailures));
        }

        // Fallback : pour les handlers qui ne suivent pas le contrat Result.
        throw new ValidationException(validationFailures);
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            // Aucun validateur enregistré pour cette requête.
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        // Exécute les validateurs en parallèle pour limiter la latence globale.
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }

    // Convertit le modèle FluentValidation vers la représentation d'erreur domaine.
    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(f => Error.Problem(f.ErrorCode, f.ErrorMessage)).ToArray());
}
