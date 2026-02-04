using App.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Entities;

public class Role : IdentityRole<Guid>, IEntityBase
{
    public Role()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}