using API.Helpers;
using Application.Commands.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationAppService _authenticationAppService;
    public AuthenticationController(IAuthenticationAppService authenticationAppService)
    {
        _authenticationAppService = authenticationAppService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserCommand command)
    {
        var token = await _authenticationAppService.Register(command);
        return Response(result: token, message: "User registered successfully");
    }
    
    [HttpPost("register/confirm")]
    public async Task<IActionResult> ConfirmRegister(ConfirmRegisterCommand command)
    {
        await _authenticationAppService.ConfirmRegister(command);
        return Response(message: "User confirmed successfully");
    }
}