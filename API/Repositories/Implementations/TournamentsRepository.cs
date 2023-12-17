using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace API.Repositories.Implementations;

public class TournamentsRepository : RepositoryBase<Tournament>, ITournamentsRepository
{
	private readonly OtrContext _context;
	private readonly ILogger<TournamentsRepository> _logger;
	private readonly IMatchesRepository _matchesRepository;

	public TournamentsRepository(ILogger<TournamentsRepository> logger, OtrContext context, IMatchesRepository matchesRepository) : base(context)
	{
		_logger = logger;
		_context = context;
		_matchesRepository = matchesRepository;
	}

	public async Task<Tournament?> GetAsync(string name) => await _context.Tournaments.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

	public async Task PopulateAndLinkAsync()
	{
		var matches = await MatchesWithoutTournamentAsync();
		foreach (var match in matches)
		{
			var associatedTournament = await AssociatedTournament(match);

			if (associatedTournament == null)
			{
				associatedTournament = await CreateFromMatchDataAsync(match);

				if (associatedTournament != null)
				{
					_logger.LogInformation("Created tournament {TournamentName} ({TournamentId})", associatedTournament?.Name, associatedTournament?.Id);
				}
			}

			if (associatedTournament == null)
			{
				_logger.LogError("Could not create tournament from match {MatchId}", match.MatchId);
				continue;
			}

			var updated = LinkTournamentToMatch(associatedTournament, match);

			await _matchesRepository.UpdateAsync(updated);
			_logger.LogInformation("Linked tournament {TournamentName} ({TournamentId}) to match {MatchId}", associatedTournament.Name, associatedTournament.Id, match.MatchId);
		}
	}

	public async Task<bool> ExistsAsync(string name, int mode) => await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Mode == mode);

	public async Task<PlayerTournamentTeamSizeCountDTO> GetPlayerTeamSizeStatsAsync(int playerId, int mode, DateTime dateMin, DateTime dateMax)
	{
		var counts = await _context.Tournaments
		                           .Where(x => x.Mode == mode)
		                           .Include(x => x.Matches.Where(y => y.StartTime >= dateMin && y.StartTime <= dateMax))
		                           .ThenInclude(x => x.Games)
		                           .ThenInclude(x => x.MatchScores.Where(y => y.PlayerId == playerId))
		                           .Select(x => x.TeamSize)
		                           .ToListAsync();

		return new PlayerTournamentTeamSizeCountDTO
		{
			Count1v1 = counts.Count(x => x == 1),
			Count2v2 = counts.Count(x => x == 2),
			Count3v3 = counts.Count(x => x == 3),
			Count4v4 = counts.Count(x => x == 4),
			CountOther = counts.Count(x => x > 4)
		};
	}

	public async Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false)
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
			              SELECT t.id as TournamentId, t.name as TournamentName, AVG(mrs.match_cost) as MatchCost
			              								FROM tournaments t
			              								INNER JOIN matches m ON m.tournament_id = t.id
			              								INNER JOIN match_rating_stats mrs ON mrs.match_id = m.id
			              								WHERE mrs.player_id = @playerId AND t.mode = @mode AND m.start_time >= @dateMin AND m.start_time <= @dateMax
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
						Mode = mode
					});
				}

				return results;
			}
		}
	}


	private async Task<Tournament> UpdateExisting(BatchWrapper wrapper)
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

	private async Task<Tournament> CreateFromWrapperAsync(BatchWrapper wrapper)
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

	private async Task<IList<Match>> MatchesWithoutTournamentAsync() => await _context
	                                                                          .Matches.Where(x =>
		                                                                          x.TournamentId == null &&
		                                                                          x.TournamentName != null &&
		                                                                          x.Abbreviation != null &&
		                                                                          x.Mode != null &&
		                                                                          x.RankRangeLowerBound != null &&
		                                                                          x.TeamSize != null)
	                                                                          .ToListAsync();

	private async Task<Tournament?> AssociatedTournament(Match match)
	{
		if (match.Abbreviation == null || match.TournamentName == null)
		{
			return null;
		}

		return await _context.Tournaments
		                     .FirstOrDefaultAsync(x =>
			                     x.Name.ToLower() == match.TournamentName.ToLower() && x.Abbreviation.ToLower() == match.Abbreviation.ToLower());
	}

	private Match LinkTournamentToMatch(Tournament t, Match m)
	{
		if (t.Id == 0)
		{
			throw new ArgumentException("Tournament must be saved to the database before it can be linked to a match.");
		}

		m.TournamentId = t.Id;
		return m;
	}

	private async Task<Tournament?> CreateFromMatchDataAsync(Match m)
	{
		if (m.TournamentName == null || m.Abbreviation == null || m.Mode == null || m.RankRangeLowerBound == null || m.TeamSize == null)
		{
			return null;
		}

		var existing = await GetAsync(m.TournamentName);
		if (existing != null)
		{
			return existing;
		}

		return await CreateAsync(new Tournament
		{
			Name = m.TournamentName,
			Abbreviation = m.Abbreviation,
			ForumUrl = m.Forum ?? string.Empty,
			Mode = m.Mode.Value,
			RankRangeLowerBound = m.RankRangeLowerBound.Value,
			TeamSize = m.TeamSize.Value
		});
	}
}