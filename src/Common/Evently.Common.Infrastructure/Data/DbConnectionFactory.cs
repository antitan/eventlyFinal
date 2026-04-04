using System.Data;
using Evently.Common.Application.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Evently.Common.Infrastructure.Data;

internal sealed class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public async Task<IDbConnection> OpenConnectionAsync()
    {
        string connectionString = configuration.GetConnectionString("Database");
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
