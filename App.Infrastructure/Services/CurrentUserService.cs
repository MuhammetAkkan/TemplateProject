using App.Application.Common;
using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Services;

/*
public class CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    // Sadece Token'dan okur (DB yok, hızlı)
    public Guid UserId => _httpContextAccessor.HttpContext?.User?.GetUserId() ?? Guid.Empty;

    // Token yoksa Guest'i DB'den bulur getirir
    public async Task<Guid> GetUserIdOrGuestAsync()
    {
        var id = UserId;
        if (id != Guid.Empty) return id;

        var guest = await userManager.FindByEmailAsync("guest@example.com");
        return guest?.Id ?? Guid.Empty;
    }
}
*/