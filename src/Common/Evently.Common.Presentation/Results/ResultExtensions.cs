using Evently.Common.Domain;

namespace Evently.Common.Presentation.Results;

/// <summary>
/// Fournit une API de pattern matching simple pour manipuler les Result.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Exécute l'action de succès ou d'échec pour un résultat non générique.
    /// </summary>
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    /// <summary>
    /// Exécute l'action de succès ou d'échec pour un résultat avec valeur.
    /// </summary>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }
}
