using API.Entities;
using API.Enums;
using API.Osu;
using Microsoft.EntityFrameworkCore;

namespace API.Utilities;

public static class QueryExtensions
{
	// Player
	public static IQueryable<Player> WhereOsuId(this IQueryable<Player> query, long osuId) => query.AsQueryable().Where(x => x.OsuId == osuId);

	// Match
	public static IQueryable<Match> WhereVerified(this IQueryable<Match> query) => query.AsQueryable().Where(x => x.VerificationStatus == (int)MatchVerificationStatus.Verified && x.IsApiProcessed == true && x.NeedsAutoCheck == false);
	public static IQueryable<Match> After(this IQueryable<Match> query, DateTime after) => query.AsQueryable().Where(x => x.StartTime > after);
	public static IQueryable<Match> Before(this IQueryable<Match> query, DateTime before) => query.AsQueryable().Where(x => x.StartTime < before);
	public static IQueryable<Match> WhereMode(this IQueryable<Match> query, int playMode) => query.AsQueryable().Where(x => x.Tournament != null && x.Tournament.Mode == playMode);

	public static IQueryable<Match> IncludeAllChildren(this IQueryable<Match> query) => query.AsQueryable()
	                                                                                         .Include(x => x.Games)
	                                                                                         .ThenInclude(x => x.MatchScores)
	                                                                                         .Include(x => x.Games)
	                                                                                         .ThenInclude(x => x.Beatmap);

	public static IQueryable<Match> WherePlayerParticipated(this IQueryable<Match> query, long osuPlayerId) => query.AsQueryable()
	                                                                                                                .Where(x =>
		                                                                                                                x.Games.Any(y =>
			                                                                                                                y.MatchScores.Any(z =>
				                                                                                                                z.Player.OsuId == osuPlayerId)));
	// Game
	public static IQueryable<Game> WhereVerified(this IQueryable<Game> query) =>
		query.AsQueryable().Where(x => x.VerificationStatus == (int)GameVerificationStatus.Verified && x.RejectionReason == null);

	public static IQueryable<Game> WhereTeamVs(this IQueryable<Game> query) => query.AsQueryable().Where(x => x.TeamType == (int)OsuEnums.TeamType.TeamVs);
	public static IQueryable<Game> WhereHeadToHead(this IQueryable<Game> query) => query.AsQueryable().Where(x => x.TeamType == (int)OsuEnums.TeamType.HeadToHead);
	public static IQueryable<Game> After(this IQueryable<Game> query, DateTime after) => query.AsQueryable().Where(x => x.StartTime > after);

	/// <summary>
	///  Returns all MatchScores where either the game or the player had the specified mods enabled
	/// </summary>
	/// <param name="query"></param>
	/// <param name="enabledMods"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereMods(this IQueryable<MatchScore> query, OsuEnums.Mods enabledMods)
	{
		return query
		       .AsQueryable()
		       .Where(x =>
			       (x.Game.Mods != 0 && x.Game.Mods == (int)enabledMods) || // Not using NF
			       (x.EnabledMods != null && x.EnabledMods.Value == (int)enabledMods) ||
			       (x.Game.Mods != 0 && x.Game.Mods == (int)(enabledMods | OsuEnums.Mods.NoFail)) || // Using NF
			       (x.EnabledMods != null && x.EnabledMods.Value == (int)(enabledMods | OsuEnums.Mods.NoFail)));
	}

	// MatchScore
	/// <summary>
	///  Selects scores that are verified
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereVerified(this IQueryable<MatchScore> query) =>
		query.AsQueryable().Where(x => x.IsValid != false && x.Game.Match.VerificationStatus == (int)MatchVerificationStatus.Verified);

	/// <summary>
	///  Selects all HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereHeadToHead(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.TeamType == (int)OsuEnums.TeamType.HeadToHead);

	public static IQueryable<MatchScore> WhereNotHeadToHead(this IQueryable<MatchScore> query) =>
		query.AsQueryable().Where(x => x.Game.TeamType != (int)OsuEnums.TeamType.HeadToHead);

	/// <summary>
	///  Selects all TeamVs match scores
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereTeamVs(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.TeamType == (int)OsuEnums.TeamType.TeamVs);

	/// <summary>
	///  Selects all match scores, other than the provided player's, that are on the opposite team as the provided player.
	///  Excludes HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <param name="osuPlayerId"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereOpponent(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable()
	                                                                                                                .Where(x =>
		                                                                                                                x.Game.MatchScores.Any(y =>
			                                                                                                                y.Player.OsuId == osuPlayerId) &&
		                                                                                                                x.Player.OsuId != osuPlayerId &&
		                                                                                                                x.Team != x.Game.MatchScores.First(y => y.Player.OsuId == osuPlayerId).Team);

	/// <summary>
	///  Selects all match scores, other than the provided player's, that are on the same team as the provided player. Excludes
	///  HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <param name="osuPlayerId"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereTeammate(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable()
	                                                                                                                .Where(x =>
		                                                                                                                x.Game.MatchScores.Any(x =>
			                                                                                                                x.Player.OsuId == osuPlayerId) &&
		                                                                                                                x.Player.OsuId != osuPlayerId &&
		                                                                                                                x.Game.TeamType !=
		                                                                                                                (int)OsuEnums.TeamType.HeadToHead &&
		                                                                                                                x.Team ==
		                                                                                                                x.Game.MatchScores.First(y =>
			                                                                                                                 y.Player.OsuId == osuPlayerId)
		                                                                                                                 .Team);
	
	public static IQueryable<MatchScore> WhereDateRange(this IQueryable<MatchScore> query, DateTime dateMin, DateTime dateMax) => query.AsQueryable().Where(x => x.Game.StartTime > dateMin && x.Game.StartTime < dateMax);

	/// <summary>
	///  Selects all MatchScores for a given playMode (e.g. mania)
	/// </summary>
	/// <param name="query"></param>
	/// <param name="playMode"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereMode(this IQueryable<MatchScore> query, int playMode) => query.AsQueryable().Where(x => x.Game.PlayMode == playMode);

	public static IQueryable<MatchScore> WhereOsuPlayerId(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	public static IQueryable<MatchScore> WherePlayerId(this IQueryable<MatchScore> query, int playerId) => query.AsQueryable().Where(x => x.PlayerId == playerId);
	public static IQueryable<MatchScore> After(this IQueryable<MatchScore> query, DateTime after) => query.AsQueryable().Where(x => x.Game.StartTime > after);

	// Rating
	public static IQueryable<Rating> WhereMode(this IQueryable<Rating> query, int playMode) => query.AsQueryable().Where(x => x.Mode == playMode);
	public static IQueryable<Rating> WhereOsuPlayerId(this IQueryable<Rating> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	public static IQueryable<Rating> OrderByMuDescending(this IQueryable<Rating> query) => query.AsQueryable().OrderByDescending(x => x.Mu);

	// Rating Histories
	public static IQueryable<RatingHistory> WhereOsuPlayerId(this IQueryable<RatingHistory> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	public static IQueryable<RatingHistory> WhereMode(this IQueryable<RatingHistory> query, int playMode) => query.AsQueryable().Where(x => x.Mode == playMode);
	public static IQueryable<RatingHistory> OrderByMuDescending(this IQueryable<RatingHistory> query) => query.AsQueryable().OrderByDescending(x => x.Mu);
	public static IQueryable<RatingHistory> After(this IQueryable<RatingHistory> query, DateTime after) => query.AsQueryable().Where(x => x.Created > after);
}