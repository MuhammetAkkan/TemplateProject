namespace App.Application.Abstractions.Security;

public record JwtResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration
);