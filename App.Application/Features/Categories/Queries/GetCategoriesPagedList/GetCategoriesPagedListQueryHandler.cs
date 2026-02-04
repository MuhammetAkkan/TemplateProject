using App.Application.Abstractions.Cache;
using App.Application.Features.Categories.Dto;
using App.Application.Interfaces.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Queries.GetCategoriesPagedList;

public class GetCategoriesPagedListQueryHandler
(ICategoryQueryRepository queryRepository, ICacheService cacheService) : IRequestHandler<GetCategoriesPagedListQuery, PagedResult<CategoryDto>>
{
    public async Task<PagedResult<CategoryDto>> Handle(GetCategoriesPagedListQuery request, CancellationToken cancellationToken)
    {
        var pagedCategories = await queryRepository.GetPagedCategoryDtosAsync(request.PageNumber, request.PageSize, cancellationToken);
        return pagedCategories;
    }
}