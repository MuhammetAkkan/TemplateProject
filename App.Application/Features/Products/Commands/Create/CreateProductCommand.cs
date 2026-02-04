using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Products.Commands.Create;

public record CreateProductCommand(string Name, string? Description, decimal Price, Guid CategoryId) : IRequest<Result<Guid>>;