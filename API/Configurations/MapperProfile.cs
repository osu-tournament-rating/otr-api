using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Configurations;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<Beatmap, BeatmapDTO>();
		CreateMap<Game, GameDTO>();
		CreateMap<GameWinRecord, GameWinRecordDTO>();
		CreateMap<Match, MatchDTO>();
		CreateMap<MatchScore, MatchScoreDTO>().ForMember(x => x.Misses, opt => opt.MapFrom(y => y.CountMiss));
		CreateMap<RatingAdjustment, RatingAdjustmentDTO>();
		CreateMap<MatchRatingStats, MatchRatingStatsDTO>()
			.ForMember(x => x.TooltipInfo, opt => opt.MapFrom(x => new MatchTooltipInfoDTO
			{
				MatchName = x.Match.Name,
				MatchDate = x.Match.StartTime,
				MpLink = $"https://osu.ppy.sh/mp/{x.Match.MatchId}",
				TournamentAbbreviation = x.Match.Tournament.Abbreviation,
				TournamentName = x.Match.Tournament.Name
			}));
		CreateMap<Player, PlayerDTO>();
		CreateMap<Player, PlayerRanksDTO>();
		CreateMap<Player, PlayerInfoDTO>();
		CreateMap<Tournament, TournamentDTO>();
		CreateMap<User, UserDTO>();
	}
}