using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers;

public abstract class BaseApiController : ControllerBase
{
    protected new IActionResult Response(object result = null, bool isValidOperation = true, string message = null)
    {
        if (isValidOperation)
        {
            return Ok(new Response<object>
            {
                Success = true,
                Message = message,
                Data = result
            });
        }

        return BadRequest(new Response<object>
        {
            Success = false,
            Errors = (Dictionary<string, List<string>>)result
        });
    }

    protected new IActionResult NotFound()
    {
        return NotFound(new Response<object>
        {
            Success = false,
            Message = "Object was not found"
        });
    }
}