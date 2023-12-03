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
		CreateMap<Match, MatchDTO>();
		CreateMap<MatchScore, MatchScoreDTO>().ForMember(x => x.Misses, opt => opt.MapFrom(y => y.CountMiss));
		CreateMap<RatingAdjustment, RatingAdjustmentDTO>();
		CreateMap<MatchRatingStats, MatchRatingStatsDTO>()
		   .ForMember(x => x.TooltipInfo, opt => opt.MapFrom(x => new MatchTooltipInfoDTO
		   {
			   MatchName = x.Match.Name,
			   MatchDate = x.Match.StartTime,
			   MpLink = $"https://osu.ppy.sh/mp/{x.Match.MatchId}",
			   TournamentAbbreviation = x.Match.Tournament != null ? x.Match.Tournament.Abbreviation : null,
			   TournamentName = x.Match.Tournament != null ? x.Match.Tournament.Name : null
		   }));
		CreateMap<Player, PlayerRanksDTO>();
		CreateMap<Tournament, TournamentDTO>();
		CreateMap<User, UserDTO>();
	}
}