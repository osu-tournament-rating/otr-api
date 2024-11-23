using API.DTOs;
using AutoMapper;

namespace API.Utilities.Extensions;

public static class MappingExtensions
{
    public static IMappingExpression<TSource, TDest> MapAsCreatedResult<TSource, TDest>
        (this IMappingExpression<TSource, TDest> mappingExpression)
        where TDest : CreatedResultBaseDTO
        => mappingExpression
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtRouteValues, opt => opt.Ignore());
}
