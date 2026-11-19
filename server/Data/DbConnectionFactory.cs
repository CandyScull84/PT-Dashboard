using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FullstackApp1.server.Data;

public sealed class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Sql")
        ?? throw new InvalidOperationException("ConnectionString:Sql saknas i appsettings.json");
    }

    public SqlConnection Create() => new SqlConnection(_connectionString);
   
}