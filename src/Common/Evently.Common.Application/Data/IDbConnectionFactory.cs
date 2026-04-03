using System.Data;

namespace Evently.Common.Application.Data;

/// <summary>
/// Fabrique de connexions SQL (abstraction pour Dapper/requêtes read-model).
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Crée une connexion ouverte prête à être utilisée.
    /// </summary>
    Task<IDbConnection> OpenConnectionAsync();
}
