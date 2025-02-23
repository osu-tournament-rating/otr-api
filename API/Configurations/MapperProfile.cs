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
        CreateMap<AdminNoteEntityBase, AdminNoteDTO>();

        CreateMap<Beatmap, BeatmapDTO>();
        CreateMap<Beatmapset, BeatmapsetCompactDTO>();
        CreateMap<BeatmapAttributes, BeatmapAttributesDTO>();

        CreateMap<Game, GameDTO>()
            .ForMember(x => x.Players, opt => opt.Ignore());
        CreateMap<GameRoster, GameWinRecordDTO>();

        CreateMap<GameScore, GameScoreDTO>();

        CreateMap<Match, MatchDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(x => x.Tournament.Ruleset))
            .ForMember(x => x.Players, opt => opt.Ignore());

        CreateMap<Match, MatchSubmissionStatusDTO>();
        CreateMap<Match, MatchCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<Match, MatchSearchResultDTO>();

        CreateMap<OAuthClient, OAuthClientDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id));
        CreateMap<OAuthClient, OAuthClientCreatedDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.ClientSecret, opt => opt.Ignore());

        CreateMap<RatingAdjustment, RatingAdjustmentDTO>();

        CreateMap<Player, PlayerCompactDTO>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(y => y.User!.Id));
        CreateMap<PlayerOsuRulesetData, PlayerOsuRulesetDataDTO>();

        CreateMap<Tournament, TournamentCompactDTO>();
        CreateMap<Tournament, TournamentDTO>();
        CreateMap<Tournament, TournamentCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<Tournament, TournamentSearchResultDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(y => y.Ruleset));

        CreateMap<User, UserCompactDTO>();
        CreateMap<User, UserDTO>();
        CreateMap<UserSettings, UserSettingsDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(us => us.DefaultRuleset))
            .ForMember(x => x.RulesetIsControlled, opt => opt.MapFrom(us => us.DefaultRulesetIsControlled));
    }
}
