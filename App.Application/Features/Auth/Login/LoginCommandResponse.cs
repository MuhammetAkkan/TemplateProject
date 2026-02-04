namespace App.Application.Features.Auth.Login;

public record LoginCommandResponse(
    string AccessToken, // "Token" yerine AccessToken daha net
    string RefreshToken,
    DateTime Expiration, // AccessToken bitiş süresi
    Guid UserId // Frontend'de hemen kullanmak için (isteğe bağlı Email de eklenebilir)
);