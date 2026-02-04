using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Auth.Login;

// Identifier: Email veya Username olabilir.
public record LoginCommand(string Identifier, string Password) : IRequest<Result<LoginCommandResponse>>;