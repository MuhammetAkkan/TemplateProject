using App.Application.Features.Products.Dto;
using App.Application.Interfaces.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Products.Queries.GetProductsPagedList;

public class GetProductsPagedListQueryHandler
(IProductQueryRepository queryRepository) : IRequestHandler<GetProductsPagedListQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsPagedListQuery request, CancellationToken cancellationToken)
    {
        return await queryRepository.GetPagedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }

    
}