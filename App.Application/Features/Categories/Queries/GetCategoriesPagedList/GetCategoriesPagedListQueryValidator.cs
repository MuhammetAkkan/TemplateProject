using FluentValidation;

namespace App.Application.Features.Categories.Queries.GetCategoriesPagedList;

public class GetCategoriesPagedListQueryValidator : AbstractValidator<GetCategoriesPagedListQuery>
{
    public GetCategoriesPagedListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu en fazla 100 olabilir.");
    }
}