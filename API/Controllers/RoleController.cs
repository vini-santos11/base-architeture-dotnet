using API.Helpers;
using Application.Commands.Role;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoleController : BaseApiController
{
    private readonly IRoleAppService _roleAppService;
    public RoleController(IRoleAppService roleAppService)
    {
        _roleAppService = roleAppService;
    }
    
    [Authorize(Policy = "ReadRole")]
    [HttpGet]
    public IActionResult Get()
    {
        var roles = _roleAppService.GetAll();
        return Response(roles);
    }
    
    [Authorize(Policy = "ReadRole")]
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var role = _roleAppService.GetById(id);
        return Response(role);
    }
    
    [Authorize(Policy = "CreateRole")]
    [HttpPost]
    public async Task<IActionResult> Post(CreateRoleCommand command)
    {
        await _roleAppService.AddRole(command);
        return Response(message: "Role created successfully");
    }
    
    //[Authorize(Policy = "UpdateRole")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, CreateRoleCommand command)
    {
        await _roleAppService.UpdateRole(id, command);
        return Response(message: "Role updated successfully");
    }
}