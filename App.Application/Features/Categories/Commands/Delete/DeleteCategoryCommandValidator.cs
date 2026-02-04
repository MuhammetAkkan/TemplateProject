using FluentValidation;

namespace App.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        // id is guid
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category Id is required.")
            .Must(id => id != Guid.Empty).WithMessage("Category Id must be a valid GUID.");
    }
}