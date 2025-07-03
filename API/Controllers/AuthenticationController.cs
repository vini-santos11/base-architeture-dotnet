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
        var result = await _authenticationAppService.Login(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserCommand command)
    {
        var result = await _authenticationAppService.Register(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("register/confirm")]
    public async Task<IActionResult> ConfirmRegister(ConfirmRegisterCommand command)
    {
        var result = await _authenticationAppService.ConfirmRegister(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("register/resend-code")]
    public async Task<IActionResult> ResendRegisterCode(RequestCodeCommand command)
    {
        var result = await _authenticationAppService.ResendRegisterCode(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("recover/password/request")]
    public async Task<IActionResult> RequestRecoverPassword(RequestCodeCommand command)
    {
        var result = await _authenticationAppService.RequestRecoverPassword(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("recover/password/check")]
    public async Task<IActionResult> CheckRecoveryCodeIsValid(CheckRecoveryCodeCommand command)
    {
        var result = await _authenticationAppService.CheckRecoveryCodeIsValid(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }

    [HttpPost("recover/password")]
    public async Task<IActionResult> RecoverPassword(RecoveryPasswordCommand command)
    {
        var result = await _authenticationAppService.RecoverPassword(command);
        return !result.Success ? StatusCode(result.StatusCode, result) : Ok(result);
    }
}