using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Services.Implementations;

/*
This service directly reimplements the functionality of "AspNetCore.Mvc.ControllerBase.CreatedAtAction()"
It would be possible to generate routes at the controller level using the mentioned method before returning,
but it better fits our chosen flow to decouple this from the controllers and into a dedicated service
*/
public class UrlHelperService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : IUrlHelperService
{
    private readonly HttpRequest? _httpRequest = actionContextAccessor.ActionContext?.HttpContext.Request;

    private readonly IUrlHelper? _urlHelper = actionContextAccessor.ActionContext != null
        ? urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext)
        : null;

    private const string UnknownText = "unknown";

    public string Action(CreatedAtRouteValues routeValues)
    {
        if (_urlHelper is null || _httpRequest is null)
        {
            return UnknownText;
        }

        return _urlHelper.Action(
            routeValues.Action,
            routeValues.Controller,
            routeValues.RouteValues,
            _httpRequest.Scheme,
            _httpRequest.Host.ToUriComponent()
        ) ?? UnknownText;
    }
}
