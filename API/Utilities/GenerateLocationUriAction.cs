using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities.Interfaces;

namespace API.Utilities;

/// <summary>
/// Generates a resource route for the given <see cref="ICreatedResult"/>, and assigns it to the Location property
/// </summary>
// Resharper suggests making the class abstract, but it is constructed via the DI container
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GenerateLocationUriAction(IUrlHelperService urlHelperService) : IMappingAction<IEntity, CreatedResultBaseDTO>
{
    public void Process(IEntity src, CreatedResultBaseDTO dest, ResolutionContext ctx)
    {
        dest.Location = urlHelperService.Action(dest.CreatedAtRouteValues);
    }
}
