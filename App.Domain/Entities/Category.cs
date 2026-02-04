using App.Domain.Common.Interfaces;

namespace App.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    // one to many relationship with Product
    public ICollection<Product> Products { get; set; } = new List<Product>();
    
}