using App.Application.Interfaces.Data;
using App.Domain.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommandHandler
(ICategoryRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            return Result.Failure("Category not found.");
        }

        repository.Delete(category);
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult;
        
        return Result.Success($"{saveResult.Value} adet kategori silindi.");
    }
}