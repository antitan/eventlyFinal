using System.Data;
using Evently.Common.Application.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; 

namespace Evently.Common.Infrastructure.Data;

internal sealed class DbConnectionFactory(
    IConfiguration configuration,
    ILogger<DbConnectionFactory> logger) : IDbConnectionFactory
{
    public async Task<IDbConnection> OpenConnectionAsync()
    {
        try
        {
            string connectionString = configuration.GetConnectionString("Database");
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "{ClassName}:{MethodName} - Erreur lors de l'ouverture de la connexion SQL Server.",
                nameof(DbConnectionFactory),
                nameof(OpenConnectionAsync));
            throw ;
        }
    }
}
