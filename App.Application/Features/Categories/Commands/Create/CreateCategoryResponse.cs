namespace App.Application.Features.Categories.Commands.Create;

public record CreateCategoryResponse(Guid Id, string Name, string? Description);