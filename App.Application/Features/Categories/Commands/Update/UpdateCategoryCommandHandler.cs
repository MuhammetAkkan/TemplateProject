using App.Application.Interfaces.Data;
using App.Domain.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Update;

public class UpdateCategoryCommandHandler
(ICategoryRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        // 1. Varlık Kontrolü
        var category =  await repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            return Error.NotFound("Kategori bulunamadı.");
        }
        // 2. Varlık Güncelleme
        category.Name = request.Name;
        category.Description = request.Description ?? category.Description;
        
        // 3. Veritabanı İşlemleri
        repository.Update(category);
        var saveResult= await unitOfWork.SaveChangesAsync(ct);

        if (saveResult.IsFailure)
            return saveResult;
        
        // 4. Dönüş (Result Pattern)
        return Result.Success($"{saveResult.Value} kategori başarıyla güncellendi.");
    }
}