using FluentValidation;

namespace App.Application.Features.Categories.Queries.GetById;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kategori Id boş olamaz.");
    }
}