using API.Enums;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;
using Moq;
using Match = API.Entities.Match;

namespace APITests.Framework.MockRepositories;

public class MockApiMatchRepository : Mock<IApiMatchRepository>
{
    public MockApiMatchRepository SetupCreateFromApiMatch()
    {
        Setup(x =>
                x.CreateFromApiMatchAsync(It.IsAny<OsuApiMatchData>())
            )
            .ReturnsAsync((OsuApiMatchData matchData) =>
            {
                // Not sure whether all of these fields need valid values at this time.
                var match = new Match
                {
                    Id = 1234,
                    MatchId = matchData.OsuApiMatch.MatchId,
                    Name = matchData.OsuApiMatch.Name,
                    StartTime = matchData.OsuApiMatch.StartTime,
                    EndTime = matchData.OsuApiMatch.EndTime,
                    VerificationInfo = null,
                    VerificationSource = null,
                    VerificationStatus = (int)MatchVerificationStatus.PendingVerification,
                    VerifierUserId = null,
                    TournamentId = 0,
                    NeedsAutoCheck = true,
                    IsApiProcessed = true,
                    SubmitterUserId = null,
                    Created = default,
                    Updated = null,
                    SubmittedBy = null,
                    VerifiedBy = null,
                    Games = null,
                    Tournament = null,
                    Stats = null,
                    RatingStats = null,
                    WinRecord = null
                };

                return match;
            });

        return this;
    }
}
