using App.Application.Features.Products.Dto;
using App.Application.Interfaces.Data;
using App.Domain.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandler
(IProductRepository productRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // mapping
        var product = new Domain.Entities.Product
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Price = request.Price,
            CategoryId = request.CategoryId
        };
        
        // add
        await productRepository.AddAsync(product, ct);
        
        // save
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<Guid>.Success(product.Id, "Product created successfully.");
        
    }
}