using App.Application.Features.Categories.Dto;
using App.Application.Features.Products.Dto;
using App.Application.Interfaces.Repositories.Common;

namespace App.Application.Interfaces.Repositories;

public interface IProductQueryRepository : IReadRepository<ProductDto>
{
    
}