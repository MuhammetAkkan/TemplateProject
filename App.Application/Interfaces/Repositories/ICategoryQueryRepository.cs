using App.Application.Features.Categories.Dto;
using App.Shared;

namespace App.Application.Interfaces.Repositories;

public interface ICategoryQueryRepository
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct);
    
    // get by id
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    // paged list
    Task<PagedResult<CategoryDto>> GetPagedCategoryDtosAsync(int pageNumber, int pageSize, CancellationToken ct);
}