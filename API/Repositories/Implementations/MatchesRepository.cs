using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Entities;
using API.Enums;
using API.Repositories.Interfaces;
using API.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

[SuppressMessage("Performance", "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class MatchesRepository(
    ILogger<MatchesRepository> logger,
    IMapper mapper,
    OtrContext context,
    IMatchDuplicateRepository matchDuplicateRepository
    ) : HistoryRepositoryBase<Match, MatchHistory>(context, mapper), IMatchesRepository
{
    private readonly OtrContext _context = context;
    private readonly IMatchDuplicateRepository _matchDuplicateRepository = matchDuplicateRepository;
    private readonly ILogger<MatchesRepository> _logger = logger;

    public override async Task<Match?> GetAsync(int id) =>
        // Get the match with all associated data
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Include(x => x.Games)
            .ThenInclude(x => x.Beatmap)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Match> UpdateVerificationStatus(int id, int? verificationStatus)
    {
        Match existing = await GetAsync(id, false) ?? throw new Exception("Match does not exist, this method assumes the match exists.");
        existing.VerificationStatus = verificationStatus;

        await UpdateAsync(existing);
        return existing;
    }

    public async Task RefreshAutomationChecks(bool invalidOnly = true)
    {
        IQueryable<Match> query = _context.Matches.Where(x => x.NeedsAutoCheck == false && x.IsApiProcessed == true);

        if (invalidOnly)
        {
            query = query.Where(x => x.VerificationStatus == (int)MatchVerificationStatus.Rejected);
        }

        await query.ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
        _logger.LogInformation("Refreshed automation checks for {Count} matches", await query.CountAsync());
    }

    public async Task<Match> GetAsync(int id, bool filterInvalidMatches = true) =>
        await MatchBaseQuery(filterInvalidMatches).FirstAsync(x => x.Id == id);

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<int> ids, bool onlyIncludeFiltered) =>
        await MatchBaseQuery(onlyIncludeFiltered).Where(x => ids.Contains(x.Id)).ToListAsync();

    private IQueryable<Match> MatchBaseQuery(bool filterInvalidMatches)
    {
        if (!filterInvalidMatches)
        {
            return _context
                .Matches.Include(x => x.Games)
                .ThenInclude(x => x.MatchScores)
                .Include(x => x.Games)
                .ThenInclude(x => x.Beatmap);
        }

        return _context
            .Matches.WhereVerified()
            .Include(x => x.Games.Where(y => y.VerificationStatus == (int)GameVerificationStatus.Verified))
            .ThenInclude(x => x.MatchScores.Where(y => y.IsValid == true))
            .Include(x => x.Games.Where(y => y.VerificationStatus == (int)GameVerificationStatus.Verified))
            .ThenInclude(x => x.Beatmap)
            .Where(x => x.Games.Count > 0)
            .OrderBy(x => x.StartTime);
    }

    public async Task<IEnumerable<int>> GetAllAsync(bool filterInvalidMatches)
    {
        IQueryable<Match> query = _context.Matches.OrderBy(m => m.StartTime).AsQueryable();

        if (filterInvalidMatches)
        {
            query = _context
                .Matches.Include(x =>
                    x.Games.Where(y => y.VerificationStatus == (int)GameVerificationStatus.Verified)
                )
                .ThenInclude(x => x.MatchScores.Where(y => y.IsValid == true))
                .Include(x =>
                    x.Games.Where(y => y.VerificationStatus == (int)GameVerificationStatus.Verified)
                )
                .ThenInclude(x => x.Beatmap)
                .Where(x => x.Games.Count > 0);
        }

        List<int> matches = await query.Select(x => x.Id).ToListAsync();

        return matches;
    }

    public async Task<Match?> GetByMatchIdAsync(long matchId) =>
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Include(x => x.Tournament)
            .FirstOrDefaultAsync(x => x.MatchId == matchId);

    public async Task<IList<Match>> GetMatchesNeedingAutoCheckAsync() =>
        // We only want api processed matches because the verification checks require the data from the API
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Include(x => x.Tournament)
            .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
            .ToListAsync();

    public async Task<Match?> GetFirstMatchNeedingApiProcessingAsync() =>
        await _context
            .Matches.Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Where(x => x.IsApiProcessed == false)
            .FirstOrDefaultAsync();

    public async Task<Match?> GetFirstMatchNeedingAutoCheckAsync() =>
        await _context
            .Matches.Include(x => x.Tournament)
            .Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .Where(x => x.NeedsAutoCheck == true && x.IsApiProcessed == true)
            .FirstOrDefaultAsync();

    public async Task<IList<Match>> GetNeedApiProcessingAsync() =>
        await _context.Matches.Where(x => x.IsApiProcessed == false).ToListAsync();

    public async Task<IEnumerable<Match>> GetAsync(IEnumerable<long> matchIds) =>
        await _context.Matches.Where(x => matchIds.Contains(x.MatchId)).ToListAsync();

    public async Task<int> UpdateVerificationStatusAsync(
        long matchId,
        MatchVerificationStatus status,
        MatchVerificationSource source,
        string? info = null
    )
    {
        Match? match = await _context.Matches.FirstOrDefaultAsync(x => x.MatchId == matchId);
        if (match == null)
        {
            _logger.LogWarning("Match {MatchId} not found (failed to update verification status)", matchId);
            return 0;
        }

        match.VerificationStatus = (int)status;
        match.VerificationSource = (int)source;
        match.VerificationInfo = info;

        _logger.LogInformation(
            "Updated verification status of match {MatchId} to {Status} (source: {Source}, info: {Info})",
            matchId,
            status,
            source,
            info
        );
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Match>> GetPlayerMatchesAsync(
        long osuId,
        int mode,
        DateTime before,
        DateTime after
    )
    {
        return await _context
            .Matches.IncludeAllChildren()
            .WherePlayerParticipated(osuId)
            .WhereMode(mode)
            .Before(before)
            .After(after)
            .ToListAsync();
    }

    public async Task<int> CountMatchWinsAsync(long osuPlayerId, int mode, DateTime fromTime)
    {
        var wins = 0;
        List<Match> matches = await _context
            .Matches.WhereVerified()
            .After(fromTime)
            .Include(x => x.Games)
            .ThenInclude(x => x.MatchScores)
            .ThenInclude(x => x.Player)
            .Where(x =>
                x.Games.Any(y => y.PlayMode == mode && y.MatchScores.Any(z => z.Player.OsuId == osuPlayerId))
            )
            .ToListAsync();

        foreach (Match? match in matches)
        {
            // For head to head (lobby size 2), calculate the winner based on score
            var pointsPlayer = 0;
            var pointsOpponent = 0;
            var team = 0;
            foreach (Game game in match.Games)
            {
                if (!game.MatchScores.Any(x => x.Player.OsuId == osuPlayerId))
                {
                    continue;
                }

                team = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Team;
            }

            foreach (Game game in match.Games)
            {
                try
                {
                    if (game.MatchScores.Count == 2)
                    {
                        // Assuming this is a 1v1...
                        if (!game.MatchScores.Any(x => x.Player.OsuId == osuPlayerId))
                        {
                            continue;
                        }

                        var playerScore = game.MatchScores.First(x => x.Player.OsuId == osuPlayerId).Score;
                        var opponentScore = game.MatchScores.First(x => x.Player.OsuId != osuPlayerId).Score;

                        if (playerScore > opponentScore)
                        {
                            pointsPlayer++;
                        }
                        else
                        {
                            pointsOpponent++;
                        }
                    }
                    else if (game.MatchScores.Count >= 4)
                    {
                        // Identify player team, sum the scores, then add points this way
                        var playerTeam = team;
                        var opponentTeam = game.MatchScores.FirstOrDefault(x => x.Team != playerTeam)?.Team;

                        var playerTeamScores = game
                            .MatchScores.Where(x => x.Team == playerTeam)
                            .Sum(x => x.Score);
                        var opponentTeamScores = game
                            .MatchScores.Where(x => x.Team == opponentTeam)
                            .Sum(x => x.Score);

                        if (playerTeamScores > opponentTeamScores)
                        {
                            pointsPlayer++;
                        }
                        else
                        {
                            pointsOpponent++;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(
                        e,
                        "Error occurred while calculating match wins for player {OsuId}",
                        osuPlayerId
                    );
                }
            }

            if (pointsPlayer > pointsOpponent)
            {
                wins++;
            }
        }

        return wins;
    }

    public async Task<int> CountMatchesPlayedAsync(long osuPlayerId, int mode, DateTime fromTime) =>
        await _context
            .MatchScores.WhereVerified()
            .WhereOsuPlayerId(osuPlayerId)
            .WhereMode(mode)
            .After(fromTime)
            .Include(x => x.Game)
            .GroupBy(x => x.Game.MatchId)
            .CountAsync();

    public async Task<double> GetWinRateAsync(long osuPlayerId, int mode, DateTime fromTime)
    {
        var played = await CountMatchesPlayedAsync(osuPlayerId, mode, fromTime);
        var won = await CountMatchWinsAsync(osuPlayerId, mode, fromTime);

        if (played == 0)
        {
            return 0;
        }

        return (double)won / played;
    }

    public async Task UpdateAsApiProcessed(Match match)
    {
        match.IsApiProcessed = true;
        await UpdateAsync(match);
    }

    public async Task UpdateAsAutoChecked(Match match)
    {
        match.NeedsAutoCheck = false;
        await UpdateAsync(match);
    }

    public async Task SetRequireAutoCheckAsync(bool invalidOnly = true)
    {
        if (invalidOnly)
        {
            await _context
                .Matches.Where(x =>
                    x.VerificationStatus != (int)MatchVerificationStatus.Verified
                    && x.VerificationStatus != (int)MatchVerificationStatus.PreVerified
                )
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
        }
        else
        {
            // Applies to all matches
            await _context.Matches.ExecuteUpdateAsync(x => x.SetProperty(y => y.NeedsAutoCheck, true));
        }
    }

    public async Task<IEnumerable<MatchIdMappingDTO>> GetIdMappingAsync()
    {
        return await _context
            .Matches.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new MatchIdMappingDTO { Id = x.Id, OsuMatchId = x.MatchId })
            .ToListAsync();
    }

    public async Task MergeDuplicatesAsync(int matchRootId)
    {
        Match root = await GetAsync(matchRootId) ?? throw new InvalidOperationException($"Failed to find corresponding match: {matchRootId}");
        if (root.IsApiProcessed != true)
        {
            throw new Exception("All matches must be API processed.");
        }

        if (root.Games.Count == 0)
        {
            throw new Exception("Root does not contain any games.");
        }

        var totalScores = root.Games.Select(x => x.MatchScores.Count).Sum();
        if (totalScores == 0)
        {
            throw new Exception("Root has no scores.");
        }

        var duplicateReferences = (await _matchDuplicateRepository.GetDuplicatesAsync(matchRootId)).ToList();
        if (duplicateReferences.Count == 0)
        {
            throw new Exception("Match does not have any detected duplicates.");
        }

        var duplicateMatches = (await GetMatchesFromDuplicatesAsync(duplicateReferences)).ToList();

        foreach (Match? duplicate in duplicateMatches)
        {
            if (root.TournamentId != duplicate.TournamentId)
            {
                throw new Exception("Tournament ids must match");
            }

            if (duplicate.IsApiProcessed != true)
            {
                throw new Exception("All matches must be API processed.");
            }

            var satisfiesNameCheck = root.Name == duplicate.Name;
            var satisfiesOsuIdCheck = root.MatchId == duplicate.MatchId;
            if (!satisfiesNameCheck && !satisfiesOsuIdCheck)
            {
                throw new Exception(
                    "Failed to satisfy preconditions. Either the name is a mismatch or the match id is a mismatch from the root."
                );
            }
        }

        // The rootId will be used when reassigning game / score data.
        var rootId = root.Id;
        foreach (Match? duplicate in duplicateMatches)
        {
            // Reassign all of the games' matchid fields.
            foreach (Game game in duplicate.Games)
            {
                game.MatchId = rootId;
                _context.Games.Update(game);
            }

            await _context.SaveChangesAsync();

            // Delete the match.
            // We don't delete the duplicate item entry because we
            // want to preserve the merged match links. This gives us
            // the ability to say "this match X was merged from Y, Z, etc."
            await DeleteAsync(duplicate.Id);

            _logger.LogInformation(
                "Updated {GamesCount} games in duplicate match {DuplicateId} to point to new root parent match {RootId}",
                duplicate.Games.Count,
                duplicate.Id,
                rootId
            );
        }
    }

    private async Task<IEnumerable<Match>> GetMatchesFromDuplicatesAsync(
        IEnumerable<MatchDuplicate> duplicates
    )
    {
        var ls = new List<Match>();
        foreach (MatchDuplicate dupe in duplicates)
        {
            Match? match = await GetByMatchIdAsync(dupe.OsuMatchId);
            if (match == null)
            {
                continue;
            }

            ls.Add(match);
        }

        return ls;
    }

    public async Task MarkSuspectedDuplicatesAsync(Match root, IEnumerable<Match> duplicates)
    {
        var rootId = root.Id;
        foreach (Match dupe in duplicates)
        {
            var duplicateXref = new MatchDuplicate
            {
                OsuMatchId = dupe.MatchId,
                SuspectedDuplicateOf = rootId
            };

            await _matchDuplicateRepository.CreateAsync(duplicateXref);
        }
    }

    public async Task VerifyDuplicatesAsync(int matchRoot, int userId, bool confirmed)
    {
        IEnumerable<MatchDuplicate> duplicates = await _matchDuplicateRepository.GetDuplicatesAsync(matchRoot);
        foreach (MatchDuplicate dupe in duplicates)
        {
            dupe.VerifiedBy = userId;
            dupe.VerifiedAsDuplicate = confirmed;

            await _matchDuplicateRepository.UpdateAsync(dupe);
        }
    }

    public async Task<IEnumerable<IList<Match>>> GetDuplicateGroupsAsync()
    {
        // Fetch groups by MatchId, excluding matches present in MatchDuplicates and confirmed duplicates
        var duplicatesById = (
            await _context
                .Matches.Where(m => !_context.MatchDuplicates.Any(md => md.OsuMatchId == m.MatchId))
                .Where(m => !_context.MatchDuplicates.Any(md => md.VerifiedAsDuplicate == true))
                .GroupBy(m => new { m.TournamentId, m.MatchId })
                .ToListAsync()
        )
            .Select(g => new { Group = g, Count = g.Count() })
            .Where(g => g.Count > 1)
            .Select(g => g.Group.ToList()) // Convert each group to List<Match>
            .ToList();

        // Fetch groups by Name and start date, excluding matches present in MatchDuplicates
        var groupedByNameAndDate = await _context
            .Matches.Where(m =>
                m.Name != null
                && m.StartTime.HasValue
                && !_context.MatchDuplicates.Any(md => md.OsuMatchId == m.MatchId)
            )
            .GroupBy(m => new
            {
                m.TournamentId,
                m.Name,
                m.StartTime!.Value.Date
            })
            .ToListAsync();

        var duplicatesByName = groupedByNameAndDate
            .Select(g => new
            {
                Group = g.Where(m1 =>
                        g.Any(m2 =>
                            m1 != m2 && Math.Abs((m2.StartTime - m1.StartTime)!.Value.TotalHours) <= 2
                        )
                    )
                    .ToList(),
                Count = g.Count()
            })
            .Where(g => g.Group.Count > 1)
            .Select(x => x.Group)
            .ToList();

        return duplicatesById.Concat(duplicatesByName);
    }
}
