using App.Domain.Entities;

namespace App.Application.Abstractions.Security;

public interface IJwtProvider
{
    // CancellationToken'a genelde token üretirken ihtiyaç olmaz ama async/await için tutabiliriz.
    Task<JwtResponse> CreateTokenAsync(User user);
}