using System.Data;
using Evently.Common.Application.Data;
using Microsoft.Data.SqlClient;

namespace Evently.Common.Infrastructure.Data;

internal sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> OpenConnectionAsync()
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
