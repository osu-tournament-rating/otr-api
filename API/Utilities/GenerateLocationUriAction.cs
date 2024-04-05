using System.Diagnostics.CodeAnalysis;
using API.DTOs.Interfaces;
using API.Entities.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Utilities;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GenerateLocationUriAction(IUrlHelperService urlHelperService) : IMappingAction<IEntity, ICreatedResult>
{
    public void Process(IEntity src, ICreatedResult dest, ResolutionContext ctx)
    {
        dest.Location = urlHelperService.Action(dest.CreatedAtRouteValues);
    }
}
