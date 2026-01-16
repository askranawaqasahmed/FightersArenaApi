using Ideageek.FightersArena.Core.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase, IActionFilter
{
    protected IActionResult ApiOk(string message, object? value = null, HttpStatusCode code = HttpStatusCode.OK) =>
        ApiResponse(false, message, value, code);

    protected IActionResult ApiCreated(string message, object? value = null) =>
        ApiResponse(false, message, value, HttpStatusCode.Created);

    protected IActionResult ApiAccepted(string message, object? value = null) =>
        ApiResponse(false, message, value, HttpStatusCode.Accepted);

    protected IActionResult ApiError(HttpStatusCode code, string message, object? value = null) =>
        ApiResponse(true, message, value, code);

    private IActionResult ApiResponse(bool error, string message, object? value, HttpStatusCode code) =>
        StatusCode((int)HttpStatusCode.OK, ResponseHandler.ResponseStatus(error, message, value, code));

    [NonAction]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();

            var message = errors.Length > 0
                ? string.Join("; ", errors!)
                : "Invalid request payload.";

            context.Result = ApiError(HttpStatusCode.BadRequest, message);
        }

        // no-op continue
    }

    [NonAction]
    public void OnActionExecuted(ActionExecutedContext context) { }
}
