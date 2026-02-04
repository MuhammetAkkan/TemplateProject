using App.Application.Features.Products.Dto;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Products.Queries.GetProductsPagedList;

public record GetProductsPagedListQuery(int PageNumber, int PageSize) : IRequest<PagedResult<ProductDto>>;