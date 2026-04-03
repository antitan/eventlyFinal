namespace Evently.Common.Application.Clock;

/// <summary>
/// Abstraction temporelle pour rendre le code testable.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Date/heure UTC courante.
    /// </summary>
    DateTime UtcNow { get; }
}
