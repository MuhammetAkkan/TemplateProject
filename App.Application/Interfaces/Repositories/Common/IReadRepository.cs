using App.Shared;

namespace App.Application.Interfaces.Repositories.Common;

public interface IReadRepository<TResponse>
{
    Task<PagedResult<TResponse>> GetPagedListAsync(int pageNumber, int pageSize, CancellationToken ct = default);
}