using App.Application.Constants;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Commands.Create;

public record CreateCategoryCommand(string Name, string? Description)
    : IRequest<Result<CreateCategoryResponse>>; // Oluşturulan ürünün Id'sini döndürüyoruz.