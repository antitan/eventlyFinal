using System.Diagnostics.CodeAnalysis;

namespace Evently.Common.Domain;

/// <summary>
/// Représente le résultat d'une opération (succès/échec) sans valeur de retour.
/// </summary>
public class Result
{
    public Result(bool isSuccess, Error error)
    {
        // Invariant important :
        // - succès => pas d'erreur
        // - échec => erreur obligatoire
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Indique si l'opération s'est terminée correctement.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Raccourci lisible pour les scénarios d'échec.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Erreur associée (Error.None en cas de succès).
    /// </summary>
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);
}

/// <summary>
/// Variante générique qui transporte une valeur en cas de succès.
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Valeur métier. Son accès n'est autorisé qu'en cas de succès.
    /// </summary>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    // Conversion pratique : une valeur non nulle devient un succès,
    // null devient un échec standardisé.
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Fabrique un échec de validation explicite (utilisé par la pipeline de validation).
    /// </summary>
    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
