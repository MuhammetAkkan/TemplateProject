using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Update;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<Result>;