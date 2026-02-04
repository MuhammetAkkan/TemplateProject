using App.Application.Features.Auth.Login;
using App.Application.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using TS.MediatR;

namespace App.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender sender) : ApiController(sender)
{
    
    // register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result =  await Sender.Send(command);
        return ResultResponse(result);
    }
    
    // login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Sender.Send(command);
        return ResultResponse(result);
    }
}