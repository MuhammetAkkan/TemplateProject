using App.Application.Interfaces.Data;
using App.Domain.Repositories;
using App.Shared;
using FluentValidation;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Create;

public class CreateCategoryCommandHandler
(ICategoryRepository repository, IUnitOfWork unitOfWork, IValidator<CreateCategoryCommand> validator) : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // exist by name kontrolü
        bool isProductExist = await repository.AnyAsync(x => x.Name == request.Name, cancellationToken);

        // 2. Entity Mapping (Daha temiz syntax)
        var category = new Domain.Entities.Category
        {
            Name = request.Name,
            // Null ise Empty ata demenin kısa yolu:
            Description = request.Description ?? string.Empty, 
          
        };
        
        // 3. Veritabanı İşlemleri
        await repository.AddAsync(category, cancellationToken);
        
        // Değişiklikleri kaydet
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // 4. Dönüş (Result Pattern)
        var response= new CreateCategoryResponse(category.Id, category.Name, category.Description ?? "");
        // Result<Guid>.Success(Data, Mesaj)
        return Result<CreateCategoryResponse>.Success(response, "Kategori başarıyla oluşturuldu.");
    }
}