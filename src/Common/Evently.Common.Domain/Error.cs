namespace Evently.Common.Domain;

/// <summary>
/// Représente une erreur métier normalisée utilisée dans tout le noyau commun.
/// </summary>
public record Error
{
    // Valeur sentinelle : utilisée quand il n'y a pas d'erreur.
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    // Valeur sentinelle : utilisée quand une valeur obligatoire est absente.
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Code stable (technique/fonctionnel) servant d'identifiant d'erreur.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Message humain destiné au diagnostic et à l'API.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Catégorie de l'erreur (validation, conflit, not found, etc.).
    /// </summary>
    public ErrorType Type { get; }

    // Helpers de construction pour conserver un style homogène dans le code métier.
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}
