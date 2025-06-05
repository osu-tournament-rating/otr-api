using API.DTOs;
using API.DTOs.Audit;
using API.Utilities;
using API.Utilities.Extensions;
using AutoMapper;
using Common.Enums;
using Database.Entities;
using Database.Entities.Processor;

namespace API.Configurations;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
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

        CreateMap<GameDTO, Game>(MemberList.Source)
            .ForMember(x => x.Beatmap, opt => opt.Ignore())
            .ForMember(x => x.Rosters, opt => opt.Ignore())
            .ForMember(x => x.Match, opt => opt.Ignore())
            .ForMember(x => x.AdminNotes, opt => opt.Ignore())
            .ForMember(x => x.IsFreeMod, opt => opt.UseDestinationValue())
            .ForSourceMember(x => x.Rosters, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.AdminNotes, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.IsFreeMod, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.Players, opt => opt.DoNotValidate());

        CreateMap<GameRoster, GameRosterDTO>();

        // Two-way mapping between entity and DTO,
        // ignores fields which should not be mapped
        // from the DTO to the entity.
        CreateMap<GameScore, GameScoreDTO>();
        CreateMap<GameScoreDTO, GameScore>(MemberList.Source)
            .ForMember(x => x.Accuracy, opt => opt.UseDestinationValue())
            .ForMember(x => x.AdminNotes, opt => opt.Ignore())
            .ForSourceMember(x => x.Accuracy, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.AdminNotes, opt => opt.DoNotValidate());

        CreateMap<Match, MatchCompactDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(x => x.Tournament.Ruleset));

        CreateMap<MatchCompactDTO, Match>(MemberList.Source)
            .ForSourceMember(x => x.Ruleset, opt => opt.DoNotValidate());

        CreateMap<Match, MatchDTO>()
            .IncludeBase<Match, MatchCompactDTO>()
            .ForMember(x => x.Players, opt => opt.Ignore());

        CreateMap<Match, MatchSubmissionStatusDTO>();
        CreateMap<Match, MatchCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();
        CreateMap<Match, MatchSearchResultDTO>()
            .ForMember(x => x.TournamentName, opt => opt.MapFrom(y => y.Tournament.Name));

        CreateMap<OAuthClient, OAuthClientDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id));
        CreateMap<OAuthClient, OAuthClientCreatedDTO>()
            .ForMember(x => x.ClientId, opt => opt.MapFrom(y => y.Id))
            .ForMember(x => x.ClientSecret, opt => opt.Ignore());

        CreateMap<RatingAdjustment, RatingAdjustmentDTO>();

        CreateMap<Player, PlayerCompactDTO>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(y => y.User!.Id));
        CreateMap<PlayerOsuRulesetData, PlayerOsuRulesetDataDTO>();

        CreateMap<PlayerTournamentStats, PlayerTournamentStatsBaseDTO>();
        CreateMap<PlayerTournamentStats, PlayerTournamentStatsDTO>()
            .IncludeBase<PlayerTournamentStats, PlayerTournamentStatsBaseDTO>();

        CreateMap<Tournament, TournamentCompactDTO>();
        CreateMap<TournamentCompactDTO, Tournament>(MemberList.Source)
            .ForMember(x => x.SubmittedByUser, opt => opt.Ignore())
            .ForMember(x => x.VerifiedByUser, opt => opt.Ignore());

        CreateMap<Tournament, TournamentDTO>();

        CreateMap<Tournament, TournamentCreatedResultDTO>()
            .MapAsCreatedResult()
            .AfterMap<GenerateLocationUriAction>();

        CreateMap<Tournament, TournamentSearchResultDTO>();

        CreateMap<User, UserCompactDTO>();
        CreateMap<User, UserDTO>();
        CreateMap<UserSettings, UserSettingsDTO>()
            .ForMember(x => x.Ruleset, opt => opt.MapFrom(us => us.DefaultRuleset))
            .ForMember(x => x.RulesetIsControlled, opt => opt.MapFrom(us => us.DefaultRulesetIsControlled));

        // Audit Logs
        CreateMap<GameAudit, AuditDTO>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ActionUserId))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ReferenceIdLock))
            .ForMember(dest => dest.Changes, opt => opt.MapFrom(src => src.Changes))
            .ForMember(dest => dest.EntityType, opt => opt.MapFrom(_ => AuditEntityType.Game));

        CreateMap<GameScoreAudit, AuditDTO>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ActionUserId))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ReferenceIdLock))
            .ForMember(dest => dest.Changes, opt => opt.MapFrom(src => src.Changes))
            .ForMember(dest => dest.EntityType, opt => opt.MapFrom(_ => AuditEntityType.GameScore));

        CreateMap<MatchAudit, AuditDTO>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ActionUserId))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ReferenceIdLock))
            .ForMember(dest => dest.Changes, opt => opt.MapFrom(src => src.Changes))
            .ForMember(dest => dest.EntityType, opt => opt.MapFrom(_ => AuditEntityType.Match));

        CreateMap<TournamentAudit, AuditDTO>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ActionUserId))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.ReferenceIdLock))
            .ForMember(dest => dest.Changes, opt => opt.MapFrom(src => src.Changes))
            .ForMember(dest => dest.EntityType, opt => opt.MapFrom(_ => AuditEntityType.Tournament));
    }
}
