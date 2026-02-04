using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Auth.Register;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<Result<RegisterCommandResponse>>;