using API.DTOs;
using API.Utilities;
using API.Utilities.Extensions;
using AutoMapper;
using Database.Entities;
using Database.Entities.Processor;

namespace API.Configurations;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Beatmap, BeatmapDTO>();
        CreateMap<Game, GameDTO>();
        CreateMap<GameWinRecord, GameWinRecordDTO>();
        CreateMap<Match, MatchDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(x => x.Tournament.Ruleset));
        CreateMap<Match, MatchSubmissionStatusDTO>();
        CreateMap<Match, MatchCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<Match, MatchSearchResultDTO>();
        CreateMap<GameScore, GameScoreDTO>().ForMember(x => x.Misses, opt => opt.MapFrom(y => y.CountMiss));

        CreateMap<OAuthClient, OAuthClientDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id));
        CreateMap<OAuthClient, OAuthClientCreatedDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.ClientSecret, opt => opt.Ignore());

        CreateMap<RatingAdjustment, RatingAdjustmentDTO>();

        CreateMap<Player, PlayerDTO>();
        CreateMap<Player, PlayerRanksDTO>();
        CreateMap<Player, PlayerInfoDTO>();
        CreateMap<Tournament, TournamentDTO>();
        CreateMap<Tournament, TournamentCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<User, UserDTO>()
            .ForMember(x => x.OsuId, opt => opt.MapFrom(y => y.Player.OsuId))
            .ForMember(x => x.Country, opt => opt.MapFrom(y => y.Player.Country))
            .ForMember(x => x.Username, opt => opt.MapFrom(y => y.Player.Username));
        CreateMap<UserSettings, UserSettingsDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(us => us.DefaultRuleset))
            .ForMember(x => x.RulesetIsControlled, opt => opt.MapFrom(us => us.DefaultRulesetIsControlled));
        CreateMap<Tournament, TournamentSearchResultDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(y => y.Ruleset));
    }
}
