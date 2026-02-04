using App.Application.Abstractions.Behaviors;
using App.Application.Features.Categories.Dto;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Queries.GetById;

// RetryPolicy attribute'u ile bu sorgu işlendiğinde 3 kez deneme yapılacak, her deneme arasında 500 ms bekleme olacak
[RetryPolicy(retryCount: 3, sleepDurationMilliseconds: 500)]
public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;