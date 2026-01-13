using Ideageek.FightersArena.Core.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ideageek.FightersArena.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ApiOk(string message, object? value = null, HttpStatusCode code = HttpStatusCode.OK) =>
        StatusCode((int)code, ResponseHandler.ResponseStatus(false, message, value, code));

    protected IActionResult ApiCreated(string message, object? value = null) =>
        StatusCode((int)HttpStatusCode.Created, ResponseHandler.ResponseStatus(false, message, value, HttpStatusCode.Created));

    protected IActionResult ApiAccepted(string message, object? value = null) =>
        StatusCode((int)HttpStatusCode.Accepted, ResponseHandler.ResponseStatus(false, message, value, HttpStatusCode.Accepted));

    protected IActionResult ApiError(HttpStatusCode code, string message, object? value = null) =>
        StatusCode((int)code, ResponseHandler.ResponseStatus(true, message, value, code));
}
