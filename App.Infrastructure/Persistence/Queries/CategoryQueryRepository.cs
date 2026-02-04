using App.Application.Features.Categories.Dto;
using App.Application.Interfaces.Data;
using App.Application.Interfaces.Repositories;
using App.Shared;
using Dapper;

namespace App.Infrastructure.Persistence.Queries;

public class CategoryQueryRepository
(IDbConnectionFactory connectionFactory) : ICategoryQueryRepository

{
    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct)
    {
        
        const string sql = @"
            SELECT 
                Id, 
                Name, 
                Description, 
                CreatedAt, 
                UpdatedAt 
            FROM Categories";

        using var connection = connectionFactory.CreateConnection();
        
        // CommandDefinition, CancellationToken'ı Dapper'a geçirmenin tek yoludur.
        var command = new CommandDefinition(sql, cancellationToken: ct);
        
        return await connection.QueryAsync<CategoryDto>(command);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
        SELECT 
            id, 
            name, 
            description, 
            created_at, 
            updated_at 
        FROM categories 
        WHERE id = @Id";

        using var connection = connectionFactory.CreateConnection();
        
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
        
        return await connection.QuerySingleOrDefaultAsync<CategoryDto>(command);
    }

    public async Task<PagedResult<CategoryDto>> GetPagedCategoryDtosAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        const string sql = @"
        SELECT 
            id, 
            name, 
            description, 
            created_at, 
            updated_at 
        FROM categories 
        ORDER BY created_at DESC
        OFFSET @Offset ROWS 
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*) FROM categories;";

        using var connection = connectionFactory.CreateConnection();
        
        var command = new CommandDefinition(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize }, cancellationToken: ct);
        
        using var multi = await connection.QueryMultipleAsync(command);
        
        var items = (await multi.ReadAsync<CategoryDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();
        
        // Doğrudan 'new' kullanmak yerine Success factory methodunu çağırıyoruz
        return PagedResult<CategoryDto>.Success(items, totalCount, pageNumber, pageSize);
    }
}