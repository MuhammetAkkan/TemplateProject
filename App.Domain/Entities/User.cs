using App.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Entities;

public class User : IdentityUser<Guid>, IEntityBase
{
    public User()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    // refresh token for maintaining user sessions
    public string? RefreshToken { get; set; }
    
    // expiration time for the refresh token
    public DateTime? RefreshTokenExpires { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}