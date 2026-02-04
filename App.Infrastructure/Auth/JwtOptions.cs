namespace App.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 30; // Varsayılan 30 dk
    public int RefreshTokenExpirationDays { get; set; } = 7;    // Varsayılan 7 gün
}