using App.Application.Abstractions.Security;
using App.Domain.Entities;
using App.Shared;
using Microsoft.AspNetCore.Identity;
using TS.MediatR;

namespace App.Application.Features.Auth.Login;

public class LoginCommandHandler(UserManager<User> userManager, IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user;

        if (request.Identifier.Contains("@"))
        {
            user = await userManager.FindByEmailAsync(request.Identifier);
        }
        else
        {
            user = await userManager.FindByNameAsync(request.Identifier);
        }

        if (user is null)
        {
            // DÜZELTME 1: <LoginCommandResponse> ekledik
            return Result<LoginCommandResponse>.Failure("Kullanıcı adı veya şifre hatalı.");
        }

        bool isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            await userManager.AccessFailedAsync(user);
            // DÜZELTME 2: <LoginCommandResponse> ekledik
            return Result<LoginCommandResponse>.Failure("Kullanıcı adı veya şifre hatalı.");
        }

        var tokenResult = await jwtProvider.CreateTokenAsync(user);

        user.RefreshToken = tokenResult.RefreshToken;
        user.RefreshTokenExpires = tokenResult.RefreshTokenExpiration;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            // DÜZELTME 3: <LoginCommandResponse> ekledik
            return Result<LoginCommandResponse>.Failure("Oturum açılırken bir hata oluştu.");
        }

        var response = new LoginCommandResponse(
            AccessToken: tokenResult.AccessToken,
            RefreshToken: tokenResult.RefreshToken,
            Expiration: tokenResult.AccessTokenExpiration,
            UserId: user.Id
        );

        // Success kısmı
        return Result<LoginCommandResponse>.Success(response);
    }
}