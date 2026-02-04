using FluentValidation;

namespace App.Application.Features.Products.Queries.GetProductsPagedList;

public class GetProductsPagedListQueryValidator : AbstractValidator<GetProductsPagedListQuery>
{
    public GetProductsPagedListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
    }
}