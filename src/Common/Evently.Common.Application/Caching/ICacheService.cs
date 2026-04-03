namespace Evently.Common.Application.Caching;

/// <summary>
/// Abstraction simple de cache distribué.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Lit une valeur typée depuis le cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stocke une valeur avec expiration optionnelle.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une entrée du cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
