using System.Text.Json.Serialization;

namespace App.Application.Features.Categories.Dto;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // desc
    public string? Description { get; set; }
    
    public string CreatedAtTime => CreatedAt.ToString("dd.MM.yyyy HH:mm");
    
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}