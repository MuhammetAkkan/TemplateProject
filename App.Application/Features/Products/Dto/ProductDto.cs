namespace App.Application.Features.Products.Dto;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    
    // foreign key
    public Guid CategoryId { get; set; }
    
    // navigation property
    public string CategoryName { get; set; } = null!;
}