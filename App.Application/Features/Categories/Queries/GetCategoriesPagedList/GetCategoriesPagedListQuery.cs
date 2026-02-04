using App.Application.Features.Categories.Dto;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Queries.GetCategoriesPagedList;

public record GetCategoriesPagedListQuery(int PageNumber, int PageSize) : IRequest<PagedResult<CategoryDto>>;