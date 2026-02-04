using System.Data;
using App.Application.Interfaces.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace App.Infrastructure.Persistence.Factories;

public class PostgreConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    // appsettings.json'da ConnectionString adının "PostgreDefaultConnection" olduğundan emin ol.
    private readonly string _connectionString = configuration.GetConnectionString("PostgreDefaultConnection")!;

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}