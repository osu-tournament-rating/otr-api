using API.DTOs;
using API.Entities;
using API.Utilities;
using AutoMapper;

namespace API.Configurations;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Matches
        CreateMap<Match, MatchDTO>()
            .ForMember(x => x.Mode, opt => opt.MapFrom(x => x.Tournament.Mode));
        CreateMap<Match, MatchCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();

        // Ratings
        CreateMap<MatchRatingStats, MatchRatingStatsDTO>()
            .ForMember(
                x => x.TooltipInfo,
                opt =>
                    opt.MapFrom(x => new MatchTooltipInfoDTO
                    {
                        MatchName = x.Match.Name,
                        MatchDate = x.Match.StartTime,
                        MpLink = $"https://osu.ppy.sh/mp/{x.Match.MatchId}",
                        TournamentAbbreviation = x.Match.Tournament.Abbreviation,
                        TournamentName = x.Match.Tournament.Name
                    })
            );

        // Tournaments
        CreateMap<Tournament, TournamentCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
    }
}
