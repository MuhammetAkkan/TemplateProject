using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Application.Abstractions.Security;
using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Auth;

public sealed class JwtProvider(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager)
    : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<JwtResponse> CreateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!)
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        
        // Options'dan gelen süreyi kullan
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessTokenExpiration,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        // Options'dan gelen süreyi kullan
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        return new JwtResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiration: accessTokenExpiration,
            RefreshTokenExpiration: refreshTokenExpiration
        );
    }
}