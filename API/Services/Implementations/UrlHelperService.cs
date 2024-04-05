using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Services.Implementations;

public class UrlHelperService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : IUrlHelperService
{
    private readonly IUrlHelper? _urlHelper = actionContextAccessor.ActionContext != null
        ? urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext)
        : null;

    public string Action(string action, string controller, object routeValues) =>
        _urlHelper?.Action(action, controller, routeValues) ?? "unknown";

    public string Action(CreatedAtRouteValues routeValues) =>
        _urlHelper?.Action(routeValues.Action, routeValues.Controller, routeValues.RouteValues) ?? "unknown";
}
