using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Delete;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;