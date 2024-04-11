using API.DTOs;
using API.Entities;
using API.Utilities;
using AutoMapper;

namespace API.Configurations;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Beatmap, BeatmapDTO>();
        CreateMap<Game, GameDTO>();
        CreateMap<GameWinRecord, GameWinRecordDTO>();
        CreateMap<Match, MatchDTO>()
            .ForMember(x => x.Mode, opt => opt.MapFrom(x => x.Tournament.Mode));
        CreateMap<Match, MatchHistory>()
            .ForMember(x => x.ReferenceId, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.HistoryStartTime, opt => opt.MapFrom(x => x.Updated))
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.HistoryAction, opt => opt.Ignore())
            .ForMember(x => x.HistoryEndTime, opt => opt.Ignore())
            .ForMember(x => x.ModifierId, opt => opt.Ignore())
            .ForMember(x => x.ReferenceMatch, opt => opt.Ignore());
        CreateMap<Match, MatchCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<MatchScore, MatchScoreDTO>().ForMember(x => x.Misses, opt => opt.MapFrom(y => y.CountMiss));
        CreateMap<OAuthClient, OAuthClientDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.ClientSecret, opt => opt.MapFrom(y => y.Secret));

        CreateMap<RatingAdjustment, RatingAdjustmentDTO>();
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

        CreateMap<Player, PlayerDTO>();
        CreateMap<Player, PlayerRanksDTO>();
        CreateMap<Tournament, TournamentDTO>();
        CreateMap<Tournament, TournamentCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<User, UserDTO>();
    }
}
