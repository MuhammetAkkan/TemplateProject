using App.Domain.Common.Interfaces;

namespace App.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // 1 product in 1 category
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}