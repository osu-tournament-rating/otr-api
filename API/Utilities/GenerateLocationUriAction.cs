using System.Diagnostics.CodeAnalysis;
using API.DTOs.Interfaces;
using API.Entities.Interfaces;
using API.Services.Interfaces;
using AutoMapper;

namespace API.Utilities;

/// <summary>
/// Generates a resource route for the given <see cref="ICreatedResult"/>, and assigns it to the Location property
/// </summary>
// Resharper suggests making the class abstract, but it is constructed via the DI container
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GenerateLocationUriAction(IUrlHelperService urlHelperService) : IMappingAction<IEntity, ICreatedResult>
{
    public void Process(IEntity src, ICreatedResult dest, ResolutionContext ctx)
    {
        dest.Location = urlHelperService.Action(dest.CreatedAtRouteValues);
    }
}
