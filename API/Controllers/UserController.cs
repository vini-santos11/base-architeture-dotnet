using API.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserAppService _userAppService;
    public UserController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    [Authorize(Policy = "CreateUser")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _userAppService.GetAll();
        return Response(users);
    }
}