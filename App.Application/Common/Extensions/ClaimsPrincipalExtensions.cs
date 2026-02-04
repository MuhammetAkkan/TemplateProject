using System.Security.Claims;

namespace App.Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        // Null check
        if (principal is null) return Guid.Empty;

        // Claim'i bul (Standart NameIdentifier veya JWT sub)
        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? principal.FindFirst("sub")?.Value;

        // Guid'e çevir
        return Guid.TryParse(claimValue, out var guid) ? guid : Guid.Empty;
    }
}