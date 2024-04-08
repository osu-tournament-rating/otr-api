using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.DTOs.Interfaces;
using API.Entities.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Utilities;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GenerateLocationUriAction(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    : IMappingAction<IEntity, ICreatedResult>
{
    private readonly IUrlHelperFactory _urlHelperFactory = urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor = actionContextAccessor;

    public void Process(IEntity src, ICreatedResult dest, ResolutionContext ctx)
    {
        if (_actionContextAccessor.ActionContext is null)
        {
            dest.Location = "unknown";
            return;
        }

        IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        CreatedAtRouteValues routeValues = dest.CreatedAtRouteValues;
        dest.Location = urlHelper.Action(routeValues.Action, routeValues.Controller, routeValues.RouteValues) ?? "unknown";
    }
}
