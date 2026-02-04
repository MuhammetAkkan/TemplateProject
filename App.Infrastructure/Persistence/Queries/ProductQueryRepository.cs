using App.Application.Features.Products.Dto;
using App.Application.Interfaces.Data;
using App.Application.Interfaces.Repositories;
using App.Shared;
using Dapper;

namespace App.Infrastructure.Persistence.Queries;

public class ProductQueryRepository(IDbConnectionFactory connectionFactory) : IProductQueryRepository
{
    public async Task<PagedResult<ProductDto>> GetPagedListAsync(int pageNumber, int pageSize,
        CancellationToken ct = default)
    {
        const string sql = @"
        SELECT 
    p.id, 
    p.name, 
    p.description, 
    p.price, 
    p.category_id AS CategoryId, -- Foreign Key
    c.name AS CategoryName,      -- Kategori tablosundan gelen isim
    p.created_at, 
    p.updated_at 
FROM products p
LEFT JOIN categories c ON p.category_id = c.id
ORDER BY p.created_at DESC
OFFSET @Offset ROWS 
FETCH NEXT @PageSize ROWS ONLY;

-- Toplam sayıyı almak için (Pagination için şart)
SELECT COUNT(*) FROM products;";

        using var connection = connectionFactory.CreateConnection();

        var command = new CommandDefinition(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize },
            cancellationToken: ct);

        using var multi = await connection.QueryMultipleAsync(command);

        var items = (await multi.ReadAsync<ProductDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return PagedResult<ProductDto>.Success(items, totalCount, pageNumber, pageSize);
    }
}