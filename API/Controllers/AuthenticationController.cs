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
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var token = await _authenticationAppService.Login(command);
        return Response(result: token, message: "User logged in successfully");
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
    
    [HttpPost("recover/password/request")]
    public async Task<IActionResult> RequestRecoverPassword(RequestCodeCommand command)
    {
        var result = await _authenticationAppService.RequestRecoverPassword(command);
        return Response(result, message: "Recover password request sent successfully");
    }
    
    [HttpPost("recover/password/check")]
    public async Task<IActionResult> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command)
    {
        var isValid = await _authenticationAppService.CheckRecoveryCodeIsValid(command);
        return isValid ? Response(result: isValid, message: "Recovery code is valid") 
                       : Response(success: false, result: isValid, message: "Recovery code is invalid");
    }
    
    [HttpPost("recover/password")]
    public async Task<IActionResult> RecoverPassword(RecoveryPasswordCommand command)
    {
        await _authenticationAppService.RecoverPassword(command);
        return Response(message: "Password recovered successfully");
    }
}