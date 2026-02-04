using System.Data;

namespace App.Application.Interfaces.Data;

public interface IDbConnectionFactory
{
    // pgsql, mysql, sqlserver vb. için bağlantı oluşturur.
    IDbConnection CreateConnection();
}