using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace API.Repositories.Implementations;

public class TournamentsRepository : RepositoryBase<Tournament>, ITournamentsRepository
{
	private readonly OtrContext _context;
	public TournamentsRepository(OtrContext context) : base(context) { _context = context; }
	public async Task<Tournament?> GetAsync(string name) => await _context.Tournaments.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
	public async Task<bool> ExistsAsync(string name, int mode) => await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Mode == mode);

	public async Task<PlayerTournamentTeamSizeCountDTO> GetPlayerTeamSizeStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		var participatedTournaments = await _context.Tournaments
		                                            .Where(tournament => tournament.Mode == mode)
		                                            .Include(tournament => tournament.Matches)
		                                            .ThenInclude(match => match.Games)
		                                            .ThenInclude(game => game.MatchScores)
		                                            .Where(tournament => tournament.Matches.Any(match =>
			                                            match.StartTime >= dateMin &&
			                                            match.StartTime <= dateMax &&
			                                            match.VerificationStatus == 0 &&
			                                            match.Games.Any(game => game.VerificationStatus == 0 &&
			                                                                    game.MatchScores.Any(score => score.PlayerId == playerId && score.IsValid == true))))
		                                            .Select(tournament => new { TournamentId = tournament.Id, TeamSize = tournament.TeamSize })
		                                            .Distinct() // Ensures each tournament is counted once
		                                            .ToListAsync();

		return new PlayerTournamentTeamSizeCountDTO
		{
			Count1v1 = participatedTournaments.Count(x => x.TeamSize == 1),
			Count2v2 = participatedTournaments.Count(x => x.TeamSize == 2),
			Count3v3 = participatedTournaments.Count(x => x.TeamSize == 3),
			Count4v4 = participatedTournaments.Count(x => x.TeamSize == 4),
			CountOther = participatedTournaments.Count(x => x.TeamSize > 4)
		};
	}

	public async Task<Tournament> CreateOrUpdateAsync(TournamentWebSubmissionDTO wrapper, bool updateExisting = false)
	{
		if (updateExisting && await ExistsAsync(wrapper.TournamentName, wrapper.Mode))
		{
			return await UpdateExisting(wrapper);
		}

		return await CreateFromWrapperAsync(wrapper);
	}

	public async Task<IEnumerable<PlayerTournamentMatchCostDTO>> GetPerformancesAsync(int count, int playerId,
		int mode, DateTime dateMin, DateTime dateMax, bool bestPerformances)
	{
		string order = bestPerformances ? "DESC" : "ASC";

		using (var command = _context.Database.GetDbConnection().CreateCommand())
		{
			string sql = $"""
			              SELECT t.id as TournamentId, t.name as TournamentName, AVG(mrs.match_cost) as MatchCost, t.abbreviation AS TournamentAcronym
			              								FROM tournaments t
			              								INNER JOIN matches m ON m.tournament_id = t.id
			              								INNER JOIN match_rating_stats mrs ON mrs.match_id = m.id
			              								WHERE mrs.player_id = @playerId AND t.mode = @mode AND m.start_time >= @dateMin AND m.start_time <= @dateMax AND m.verification_status = 0
			              								GROUP BY t.id
			              								ORDER BY AVG(mrs.match_cost) {order}
			              								LIMIT @count
			              """;

			command.CommandType = CommandType.Text;
			command.CommandText = sql;

			command.Parameters.Add(new NpgsqlParameter<int>("playerId", playerId));
			command.Parameters.Add(new NpgsqlParameter<int>("mode", mode));
			command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMin", NpgsqlDbType.TimestampTz) { Value = dateMin });
			command.Parameters.Add(new NpgsqlParameter<DateTime>("dateMax", NpgsqlDbType.TimestampTz) { Value = dateMax });
			command.Parameters.Add(new NpgsqlParameter<int>("count", count));

			await _context.Database.OpenConnectionAsync();

			using (var result = await command.ExecuteReaderAsync())
			{
				var results = new List<PlayerTournamentMatchCostDTO>();
				while (await result.ReadAsync())
				{
					results.Add(new PlayerTournamentMatchCostDTO
					{
						PlayerId = playerId,
						TournamentId = result.GetInt32(0),
						TournamentName = result.GetString(1),
						MatchCost = result.GetDouble(2),
						TournamentAcronym = result.GetString(3),
						Mode = mode
					});
				}

				return results;
			}
		}
	}

	public async Task<int> CountPlayedAsync(int playerId, int mode, DateTime? dateMin = null, DateTime? dateMax = null)
	{
		dateMin ??= DateTime.MinValue;
		dateMax ??= DateTime.MaxValue;

		return await _context.Tournaments
		                     .Include(tournament => tournament.Matches)
		                     .ThenInclude(match => match.Games)
		                     .ThenInclude(game => game.MatchScores)
		                     .Where(tournament => tournament.Mode == mode &&
		                                          tournament.Matches.Any(match =>
			                                          match.StartTime >= dateMin &&
			                                          match.StartTime <= dateMax &&
			                                          match.VerificationStatus == (int)MatchVerificationStatus.Verified &&
			                                          match.Games.Any(game => game.MatchScores.Any(score => score.PlayerId == playerId && score.IsValid == true))))
		                     .Select(x => x.Id)
		                     .Distinct()
		                     .CountAsync();
	}

	private async Task<Tournament> UpdateExisting(TournamentWebSubmissionDTO wrapper)
	{
		var existing = await GetAsync(wrapper.TournamentName);

		if (existing == null)
		{
			throw new Exception("Tournament does not exist, this method assumes the tournament exists.");
		}

		existing.Abbreviation = wrapper.Abbreviation;
		existing.ForumUrl = wrapper.ForumPost;
		existing.Mode = wrapper.Mode;
		existing.RankRangeLowerBound = wrapper.RankRangeLowerBound;
		existing.TeamSize = wrapper.TeamSize;

		await UpdateAsync(existing);
		return existing;
	}

	private async Task<Tournament> CreateFromWrapperAsync(TournamentWebSubmissionDTO wrapper)
	{
		var tournament = new Tournament
		{
			Name = wrapper.TournamentName,
			Abbreviation = wrapper.Abbreviation,
			ForumUrl = wrapper.ForumPost,
			Mode = wrapper.Mode,
			RankRangeLowerBound = wrapper.RankRangeLowerBound,
			TeamSize = wrapper.TeamSize
		};

		var result = await CreateAsync(tournament);
		if (result == null)
		{
			throw new Exception("Tournament could not be created.");
		}

		return result;
	}
}