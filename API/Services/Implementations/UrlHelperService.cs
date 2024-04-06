using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Services.Implementations;

public class UrlHelperService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : IUrlHelperService
{
    private readonly HttpRequest? _httpRequest = actionContextAccessor.ActionContext?.HttpContext.Request;

    private readonly IUrlHelper? _urlHelper = actionContextAccessor.ActionContext != null
        ? urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext)
        : null;

    private const string UnknownText = "unknown";

    public string Action(string action, string controller, object routeValues)
    {
        if (_urlHelper is null || _httpRequest is null)
        {
            return UnknownText;
        }

        return _urlHelper.Action(
            action,
            controller,
            routeValues,
            _httpRequest.Scheme,
            _httpRequest.Host.ToUriComponent()
        ) ?? UnknownText;
    }

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
