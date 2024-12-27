using API.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClaimController : BaseApiController
{
    private readonly IClaimAppService _claimAppService;
    public ClaimController(IClaimAppService claimAppService)
    {
        _claimAppService = claimAppService;
    }
    
    //[Authorize(Policy = "ReadClaim")]
    [HttpGet]
    public IActionResult Get()
    {
        var claims = _claimAppService.GetClaims();
        return Response(claims);
    }
}