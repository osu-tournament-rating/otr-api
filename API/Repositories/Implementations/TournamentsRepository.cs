using API.Controllers;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class TournamentsRepository : RepositoryBase<Tournament>, ITournamentsRepository
{
	private readonly ILogger<TournamentsRepository> _logger;
	private readonly OtrContext _context;
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

	public async Task<bool> ExistsAsync(string name, int mode)
	{
		return await _context.Tournaments.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Mode == mode);
	}
	
	public async Task<Tournament> CreateOrUpdateAsync(BatchWrapper wrapper, bool updateExisting = false)
	{
		if (updateExisting && await ExistsAsync(wrapper.TournamentName, wrapper.Mode))
		{
			return await UpdateExisting(wrapper);
		}
		
		return await CreateFromWrapperAsync(wrapper);
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

	private async Task<IList<Match>> MatchesWithoutTournamentAsync()
	{
		return await _context.Matches.Where(x => x.TournamentId == null && x.TournamentName != null && x.Abbreviation != null && x.Mode != null && x.RankRangeLowerBound != null && x.TeamSize != null)
		                     .ToListAsync();
	}

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