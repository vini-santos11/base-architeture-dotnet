using System.Net;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class ValidationExceptionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        
        var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
            .SelectMany(v => v.Errors)
            .Select(v => v.ErrorMessage)
            .ToList();

        var responseObj = new Response<object>()
        {
            Message = "Invalid body request",
            Errors = new Dictionary<string, List<string>> { { "Validation", errors } },
            StatusCode = (int)HttpStatusCode.BadRequest,
        };

        context.Result = new JsonResult(responseObj)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}