using App.Domain.Entities;
using App.Shared;
using Microsoft.AspNetCore.Identity;
using TS.MediatR;

namespace App.Application.Features.Auth.Register;

public class RegisterCommandHandler(UserManager<User>userManager) : IRequestHandler<RegisterCommand, Result<RegisterCommandResponse>>
{
    public async Task<Result<RegisterCommandResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Email ile kullanıcı var mı kontrol et
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Result<RegisterCommandResponse>.Failure("User with this email already exists.");
        }
        
        // Username ile kullanıcı var mı kontrol et
        existingUser = await userManager.FindByNameAsync(request.Username);
        
        if (existingUser is not null)
        {
            return Result<RegisterCommandResponse>.Failure("User with this username already exists.");
        }
        
        var newUser = new User
        {
            UserName = request.Username,
            Email = request.Email
        };
        
        var createResult = await userManager.CreateAsync(newUser, request.Password);
        
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<RegisterCommandResponse>.Failure($"User creation failed: {errors}");
        }
        
        var response = new RegisterCommandResponse(newUser.Id, newUser.UserName, newUser.Email);
        
        return Result<RegisterCommandResponse>.Success(response);
        
    }
}