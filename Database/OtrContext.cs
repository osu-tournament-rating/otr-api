using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Database;

/// <summary>
/// Entity Framework database context for the OTR application
/// </summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class OtrContext(DbContextOptions<OtrContext> options) : DbContext(options)
{
    /// <summary>
    /// SQL function for getting the current timestamp
    /// </summary>
    private const string SqlCurrentTimestamp = "CURRENT_TIMESTAMP";

    /// <summary>
    /// SQL formatted date to be used as a placeholder for date columns
    /// </summary>
    /// <remarks>This is the (approx) creation date of osu! :D</remarks>
    private const string SqlPlaceholderDate = "'2007-09-17T00:00:00'::timestamp";

    public virtual DbSet<Beatmap> Beatmaps { get; set; }
    public virtual DbSet<BeatmapAttributes> BeatmapAttributes { get; set; }
    public virtual DbSet<Beatmapset> Beatmapsets { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameAdminNote> GameAdminNotes { get; set; }
    public virtual DbSet<GameAudit> GameAudits { get; set; }
    public virtual DbSet<GameScore> GameScores { get; set; }
    public virtual DbSet<GameScoreAdminNote> GameScoreAdminNotes { get; set; }
    public virtual DbSet<GameScoreAudit> GameScoreAudits { get; set; }
    public virtual DbSet<GameRoster> GameRosters { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchAdminNote> MatchAdminNotes { get; set; }
    public virtual DbSet<MatchAudit> MatchAudits { get; set; }
    public virtual DbSet<MatchRoster> MatchRosters { get; set; }
    public virtual DbSet<OAuthClient> OAuthClients { get; set; }
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<PlayerHighestRanks> PlayerHighestRanks { get; set; }
    public virtual DbSet<PlayerAdminNote> PlayerAdminNotes { get; set; }
    public virtual DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
    public virtual DbSet<PlayerTournamentStats> PlayerTournamentStats { get; set; }
    public virtual DbSet<PlayerRating> PlayerRatings { get; set; }
    public virtual DbSet<RatingAdjustment> RatingAdjustments { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<TournamentAdminNote> TournamentAdminNotes { get; set; }
    public virtual DbSet<TournamentAudit> TournamentAudits { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beatmap>(entity =>
        {
            entity.Property(b => b.Id).UseIdentityAlwaysColumn();

            entity.Property(b => b.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(b => b.HasData).HasDefaultValue(true);

            // Relation: BeatmapAttributes
            entity
                .HasMany(b => b.Attributes)
                .WithOne(ba => ba.Beatmap)
                .HasForeignKey(ba => ba.BeatmapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Beatmapset
            entity
                .HasOne(b => b.Beatmapset)
                .WithMany(bs => bs.Beatmaps)
                .HasForeignKey(b => b.BeatmapsetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Games
            entity
                .HasMany(b => b.Games)
                .WithOne(g => g.Beatmap)
                .HasForeignKey(g => g.BeatmapId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Players
            entity
                .HasMany(b => b.Creators)
                .WithMany(p => p.CreatedBeatmaps)
                .UsingEntity("join_beatmap_creators");

            entity.HasIndex(b => b.OsuId).IsUnique();
        });

        modelBuilder.Entity<BeatmapAttributes>(entity =>
        {
            entity.Property(ba => ba.Id).UseIdentityAlwaysColumn();

            entity.Property(ba => ba.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Beatmap
            entity
                .HasOne(ba => ba.Beatmap)
                .WithMany(b => b.Attributes)
                .HasForeignKey(ba => ba.BeatmapId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ba => new { ba.BeatmapId, ba.Mods }).IsUnique();
        });

        modelBuilder.Entity<Beatmapset>(entity =>
        {
            entity.Property(b => b.Id).UseIdentityAlwaysColumn();

            entity.Property(b => b.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player (Creator)
            entity
                .HasOne(b => b.Creator)
                .WithMany(p => p.CreatedBeatmapsets)
                .HasForeignKey(b => b.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Beatmaps
            entity
                .HasMany(b => b.Beatmaps)
                .WithOne(bm => bm.Beatmapset)
                .HasForeignKey(bm => bm.BeatmapsetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(b => b.OsuId).IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(g => g.Id).UseIdentityAlwaysColumn();

            entity.Property(g => g.VerificationStatus).HasDefaultValue(VerificationStatus.None);
            entity.Property(g => g.RejectionReason).HasDefaultValue(GameRejectionReason.None);
            entity.Property(g => g.WarningFlags).HasDefaultValue(GameWarningFlags.None);
            entity.Property(g => g.ProcessingStatus).HasDefaultValue(GameProcessingStatus.NeedsAutomationChecks);

            entity.Property(g => g.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(g => g.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(g => g.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

            entity.Property(g => g.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

            // Relation: Audits
            entity
                .HasMany(g => g.Audits);

            // Relation: Match
            entity
                .HasOne(g => g.Match)
                .WithMany(m => m.Games)
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Beatmap
            entity
                .HasOne(g => g.Beatmap)
                .WithMany(b => b.Games)
                .HasForeignKey(g => g.BeatmapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: GameRoster
            entity
                .HasMany(g => g.Rosters)
                .WithOne(gr => gr.Game)
                .HasForeignKey(gr => gr.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: GameScore
            entity
                .HasMany(g => g.Scores)
                .WithOne(gs => gs.Game)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(g => g.MatchId);
            entity.HasIndex(g => g.StartTime);
            entity.HasIndex(g => g.OsuId).IsUnique();
        });

        modelBuilder.Entity<GameAdminNote>(entity =>
        {
            entity.Property(gan => gan.Id).UseIdentityAlwaysColumn();

            entity.Property(gan => gan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relationship: Game
            entity.HasOne(gan => gan.Game)
                .WithMany(g => g.AdminNotes)
                .HasForeignKey(gan => gan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: User
            entity.HasOne(gan => gan.AdminUser)
                .WithMany(u => u.GameAdminNotes)
                .HasForeignKey(gan => gan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GameAudit>(entity =>
        {
            entity.Property(ga => ga.Id).UseIdentityAlwaysColumn();

            entity.Property(ga => ga.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(ga => ga.ActionType);

            entity.Property(ga => ga.Changes).HasColumnType("jsonb");

            // Relation: Game
            entity
                .HasOne<Game>()
                .WithMany(g => g.Audits)
                .HasForeignKey(ga => ga.ReferenceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for API queries
            entity.HasIndex(ga => ga.ReferenceIdLock);
            entity.HasIndex(ga => ga.ActionUserId);
            entity.HasIndex(ga => ga.Created);
            entity.HasIndex(ga => new { ga.ActionUserId, ga.Created });
        });

        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.Property(gs => gs.Id).UseIdentityAlwaysColumn();

            entity.Property(gs => gs.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(gs => gs.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

            // Relation: Audits
            entity
                .HasMany(gs => gs.Audits);

            // Relation: Game
            entity
                .HasOne(gs => gs.Game)
                .WithMany(g => g.Scores)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Player
            entity
                .HasOne(gs => gs.Player)
                .WithMany(p => p.Scores)
                .HasForeignKey(gs => gs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Admin Notes
            entity
                .HasMany(gs => gs.AdminNotes)
                .WithOne(an => an.Score)
                .HasForeignKey(an => an.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(gs => gs.PlayerId);
            entity.HasIndex(gs => new { gs.PlayerId, gs.GameId }).IsUnique();
        });

        modelBuilder.Entity<GameScoreAdminNote>(entity =>
        {
            entity.Property(gsan => gsan.Id).UseIdentityAlwaysColumn();

            entity.Property(gsan => gsan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Score
            entity.HasOne(gsan => gsan.Score)
                .WithMany(gs => gs.AdminNotes)
                .HasForeignKey(gsan => gsan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity.HasOne(gsan => gsan.AdminUser)
                .WithMany(gs => gs.GameScoreAdminNotes)
                .HasForeignKey(gsan => gsan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GameScoreAudit>(entity =>
        {
            entity.Property(gsa => gsa.Id).UseIdentityAlwaysColumn();

            entity.Property(gsa => gsa.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(gsa => gsa.ActionType);

            entity.Property(gsa => gsa.Changes).HasColumnType("jsonb");

            // Relation: GameScore
            entity
                .HasOne<GameScore>()
                .WithMany(gs => gs.Audits)
                .HasForeignKey(gsa => gsa.ReferenceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for API queries
            entity.HasIndex(gsa => gsa.ReferenceIdLock);
            entity.HasIndex(gsa => gsa.ActionUserId);
            entity.HasIndex(gsa => gsa.Created);
            entity.HasIndex(gsa => new { gsa.ActionUserId, gsa.Created });
        });

        modelBuilder.Entity<GameRoster>(entity =>
        {
            entity.Property(gwr => gwr.Id).UseIdentityAlwaysColumn();

            entity.Property(gwr => gwr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Game
            entity
                .HasOne(gr => gr.Game)
                .WithMany(g => g.Rosters)
                .HasForeignKey(gr => gr.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Roster);
            entity.HasIndex(x => x.GameId);

            entity.HasIndex(x => new { x.GameId, x.Roster }).IsUnique();
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(m => m.Id).UseIdentityAlwaysColumn();

            entity.Property(m => m.Name).HasDefaultValue(string.Empty);
            entity.Property(m => m.VerificationStatus).HasDefaultValue(VerificationStatus.None);
            entity.Property(m => m.RejectionReason).HasDefaultValue(MatchRejectionReason.None);
            entity.Property(m => m.WarningFlags).HasDefaultValue(MatchWarningFlags.None);
            entity.Property(m => m.ProcessingStatus).HasDefaultValue(MatchProcessingStatus.NeedsData);

            entity.Property(m => m.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(m => m.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

            // Relation: Audits
            entity.HasMany(m => m.Audits);

            // Relation: Tournament
            entity
                .HasOne(m => m.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User (Submitter)
            entity
                .HasOne(m => m.SubmittedByUser)
                .WithMany(u => u.SubmittedMatches)
                .HasForeignKey(m => m.SubmittedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: User (Verifier)
            entity
                .HasOne(m => m.VerifiedByUser)
                .WithMany()
                .HasForeignKey(m => m.VerifiedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: MatchRoster
            entity
                .HasMany(m => m.Rosters)
                .WithOne(mr => mr.Match)
                .HasForeignKey(mr => mr.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Games
            entity
                .HasMany(m => m.Games)
                .WithOne(g => g.Match)
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: RatingAdjustment
            entity
                .HasMany(m => m.PlayerRatingAdjustments)
                .WithOne(ra => ra.Match)
                .HasForeignKey(ra => ra.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: PlayerMatchStats
            entity
                .HasMany(m => m.PlayerMatchStats)
                .WithOne(pms => pms.Match)
                .HasForeignKey(pms => pms.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Admin Notes
            entity
                .HasMany(m => m.AdminNotes)
                .WithOne(an => an.Match)
                .HasForeignKey(an => an.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => m.OsuId).IsUnique();
        });

        modelBuilder.Entity<MatchAdminNote>(entity =>
        {
            entity.Property(tan => tan.Id).UseIdentityAlwaysColumn();

            entity.Property(tan => tan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Match
            entity.HasOne(man => man.Match)
                .WithMany(m => m.AdminNotes)
                .HasForeignKey(m => m.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity.HasOne(man => man.AdminUser)
                .WithMany(u => u.MatchAdminNotes)
                .HasForeignKey(m => m.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchAudit>(entity =>
        {
            entity.Property(ma => ma.Id).UseIdentityAlwaysColumn();

            entity.Property(ma => ma.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(ma => ma.ActionType);

            entity.Property(ma => ma.Changes).HasColumnType("jsonb");

            // Relation: Match
            entity
                .HasOne<Match>()
                .WithMany(m => m.Audits)
                .HasForeignKey(ma => ma.ReferenceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for API queries
            entity.HasIndex(ma => ma.ReferenceIdLock);
            entity.HasIndex(ma => ma.ActionUserId);
            entity.HasIndex(ma => ma.Created);
            entity.HasIndex(ma => new { ma.ActionUserId, ma.Created });
        });

        modelBuilder.Entity<MatchRoster>(entity =>
        {
            entity.Property(mr => mr.Id).UseIdentityAlwaysColumn();

            entity.Property(mr => mr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Match
            entity
                .HasOne(mr => mr.Match)
                .WithMany(m => m.Rosters)
                .HasForeignKey(mr => mr.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(mr => mr.Roster);
            entity.HasIndex(mr => mr.MatchId);

            entity.HasIndex(mr => new { mr.MatchId, mr.Roster }).IsUnique();
        });

        modelBuilder.Entity<OAuthClient>(entity =>
        {
            entity.Property(c => c.Id).UseIdentityAlwaysColumn();

            entity.Property(c => c.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Admin Notes
            entity
                .HasMany(c => c.AdminNotes)
                .WithOne(an => an.OAuthClient)
                .HasForeignKey(an => an.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity
                .HasOne(c => c.User)
                .WithMany(u => u.Clients)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OAuthClientAdminNote>(entity =>
        {
            entity.Property(oacan => oacan.Id).UseIdentityAlwaysColumn();

            entity.Property(oacan => oacan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: OAuthClient
            entity
                .HasOne(oacan => oacan.OAuthClient)
                .WithMany(oac => oac.AdminNotes)
                .HasForeignKey(oacan => oacan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(p => p.Id).UseIdentityAlwaysColumn();

            entity.Property(p => p.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(p => p.Username).HasDefaultValue(string.Empty);
            entity.Property(p => p.Country).HasDefaultValue(string.Empty);
            entity.Property(p => p.DefaultRuleset).HasDefaultValue(Ruleset.Osu);
            entity.Property(p => p.OsuLastFetch).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(p => p.OsuTrackLastFetch).HasDefaultValueSql(SqlPlaceholderDate);

            // Relation: RulesetData
            entity
                .HasMany(p => p.RulesetData)
                .WithOne(rd => rd.Player)
                .HasForeignKey(rd => rd.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity
                .HasOne(e => e.User)
                .WithOne(u => u.Player)
                .HasForeignKey<User>(u => u.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: PlayerRating
            entity
                .HasMany(p => p.Ratings)
                .WithOne(pr => pr.Player)
                .HasForeignKey(pr => pr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: PlayerHighestRanks
            entity
                .HasMany(p => p.HighestRanks)
                .WithOne(pr => pr.Player)
                .HasForeignKey(pr => pr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Admin Notes
            entity
                .HasMany(m => m.AdminNotes)
                .WithOne(an => an.Player)
                .HasForeignKey(r => r.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: RatingAdjustments
            entity
                .HasMany(e => e.RatingAdjustments)
                .WithOne(ra => ra.Player)
                .HasForeignKey(ra => ra.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: GameScores
            entity
                .HasMany(p => p.Scores)
                .WithOne(ms => ms.Player)
                .HasForeignKey(ms => ms.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: MatchStats
            entity
                .HasMany(p => p.MatchStats)
                .WithOne(ms => ms.Player)
                .HasForeignKey(ms => ms.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: TournamentStats
            entity
                .HasMany(p => p.TournamentStats)
                .WithOne(pts => pts.Player)
                .HasForeignKey(pts => pts.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.OsuId).IsUnique();
            entity.HasIndex(p => p.Country);
        });

        modelBuilder.Entity<PlayerMatchStats>(entity =>
        {
            entity.Property(pms => pms.Id).UseIdentityAlwaysColumn();

            entity.Property(pms => pms.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(e => e.Player)
                .WithMany(e => e.MatchStats)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Match
            entity
                .HasOne(pms => pms.Match)
                .WithMany(m => m.PlayerMatchStats)
                .HasForeignKey(pms => pms.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pms => pms.PlayerId);
            entity.HasIndex(pms => new { pms.PlayerId, pms.Won });
            entity.HasIndex(pms => new { pms.PlayerId, pms.MatchId }).IsUnique();
        });

        modelBuilder.Entity<PlayerOsuRulesetData>(entity =>
        {
            entity.Property(rd => rd.Id).UseIdentityAlwaysColumn();

            entity.Property(rd => rd.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(rd => rd.Player)
                .WithMany(p => p.RulesetData)
                .HasForeignKey(rd => rd.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rd => new { rd.PlayerId, rd.Ruleset }).IsUnique();
            entity.HasIndex(rd => new { rd.PlayerId, rd.Ruleset, rd.GlobalRank }).IsUnique();
        });

        modelBuilder.Entity<PlayerTournamentStats>(entity =>
        {
            entity.Property(pts => pts.Id).UseIdentityAlwaysColumn();

            entity.Property(pts => pts.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(pts => pts.Player)
                .WithMany(p => p.TournamentStats)
                .HasForeignKey(pts => pts.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Tournament
            entity
                .HasOne(pts => pts.Tournament)
                .WithMany(t => t.PlayerTournamentStats)
                .HasForeignKey(pts => pts.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pts => new { pts.PlayerId, pts.TournamentId }).IsUnique();
        });

        modelBuilder.Entity<PlayerHighestRanks>(entity =>
        {
            entity.Property(pr => pr.Id).UseIdentityAlwaysColumn();

            entity.Property(pr => pr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(pr => pr.Player)
                .WithMany(p => p.HighestRanks)
                .HasForeignKey(pr => pr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pr => pr.GlobalRank).IsDescending(true);
            entity.HasIndex(pr => pr.CountryRank).IsDescending(true);
            entity.HasIndex(pr => new { pr.PlayerId, pr.Ruleset }).IsUnique();
        });

        modelBuilder.Entity<PlayerAdminNote>(entity =>
        {
            entity.Property(pan => pan.Id).UseIdentityAlwaysColumn();

            entity.Property(pan => pan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity.HasOne(pan => pan.Player)
                .WithMany(p => p.AdminNotes)
                .HasForeignKey(pan => pan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity.HasOne(pan => pan.AdminUser)
                .WithMany(u => u.PlayerAdminNotes)
                .HasForeignKey(pan => pan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlayerRating>(entity =>
        {
            entity.Property(pr => pr.Id).UseIdentityAlwaysColumn();

            entity.Property(pr => pr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(pr => pr.Player)
                .WithMany(p => p.Ratings)
                .HasForeignKey(pr => pr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pr => pr.Ruleset);
            entity.HasIndex(pr => pr.PlayerId);
            entity.HasIndex(pr => pr.Rating).IsDescending(true);         // ruleset, rating
            entity.HasIndex(pr => new { pr.Ruleset, pr.Rating }).IsDescending(false, true);
            entity.HasIndex(pr => new { pr.PlayerId, pr.Ruleset }).IsUnique();
        });

        modelBuilder.Entity<RatingAdjustment>(entity =>
        {
            entity.Property(ra => ra.Id).UseIdentityAlwaysColumn();

            entity.Property(ra => ra.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: PlayerRating
            entity
                .HasOne(ra => ra.PlayerRating)
                .WithMany(pr => pr.Adjustments)
                .HasForeignKey(ra => ra.PlayerRatingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Player
            entity
                .HasOne(ra => ra.Player)
                .WithMany(p => p.RatingAdjustments)
                .HasForeignKey(ra => ra.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Match
            entity
                .HasOne(ra => ra.Match)
                .WithMany(m => m.PlayerRatingAdjustments)
                .HasForeignKey(ra => ra.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ra => new { ra.PlayerId, ra.Timestamp });
            entity.HasIndex(ra => new { ra.PlayerId, ra.MatchId }).IsUnique();
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.Property(t => t.Id).UseIdentityAlwaysColumn();

            entity.Property(t => t.VerificationStatus).HasDefaultValue(VerificationStatus.None);
            entity.Property(t => t.RejectionReason).HasDefaultValue(TournamentRejectionReason.None);
            entity.Property(t => t.ProcessingStatus).HasDefaultValue(TournamentProcessingStatus.NeedsApproval);

            entity.Property(t => t.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(t => t.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

            // Relation: User (Submitter)
            entity
                .HasOne(t => t.SubmittedByUser)
                .WithMany(u => u.SubmittedTournaments)
                .HasForeignKey(t => t.SubmittedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: User (Verifier)
            entity
                .HasOne(t => t.VerifiedByUser)
                .WithMany()
                .HasForeignKey(t => t.VerifiedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Matches
            entity
                .HasMany(t => t.Matches)
                .WithOne(m => m.Tournament)
                .HasForeignKey(m => m.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: PlayerTournamentStats
            entity
                .HasMany(t => t.PlayerTournamentStats)
                .WithOne(pts => pts.Tournament)
                .HasForeignKey(pts => pts.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Admin Notes
            entity
                .HasMany(t => t.AdminNotes)
                .WithOne(an => an.Tournament)
                .HasForeignKey(an => an.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Pooled beatmaps
            entity
                .HasMany(t => t.PooledBeatmaps)
                .WithMany(pb => pb.TournamentsPooledIn)
                .UsingEntity("join_pooled_beatmaps");

            // Relation: Audits
            entity
                .HasMany(t => t.Audits);

            entity.HasIndex(t => t.Ruleset);
            entity.HasIndex(t => new { t.Name, t.Abbreviation }).IsUnique();
        });

        modelBuilder.Entity<TournamentAdminNote>(entity =>
        {
            entity.Property(tan => tan.Id).UseIdentityAlwaysColumn();

            entity.Property(tan => tan.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Tournament
            entity
                .HasOne(tan => tan.Tournament)
                .WithMany(t => t.AdminNotes)
                .HasForeignKey(tan => tan.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: User
            entity
                .HasOne(tan => tan.AdminUser)
                .WithMany(u => u.TournamentAdminNotes)
                .HasForeignKey(tan => tan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TournamentAudit>(entity =>
        {
            entity.Property(ta => ta.Id).UseIdentityAlwaysColumn();

            entity.Property(ta => ta.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(ta => ta.ActionType);

            entity.Property(ta => ta.Changes).HasColumnType("jsonb");

            // Relation: Tournament
            entity
                .HasOne<Tournament>()
                .WithMany(t => t.Audits)
                .HasForeignKey(ta => ta.ReferenceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for API queries
            entity.HasIndex(ta => ta.ReferenceIdLock);
            entity.HasIndex(ta => ta.ActionUserId);
            entity.HasIndex(ta => ta.Created);
            entity.HasIndex(ta => new { ta.ActionUserId, ta.Created });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Id).UseIdentityColumn();
            entity.Property(u => u.Scopes).HasDefaultValue(Array.Empty<string>());
            entity.Property(u => u.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(u => u.LastLogin)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql(SqlCurrentTimestamp);

            entity.HasIndex(u => u.PlayerId);

            // Relation: UserSettings
            entity
                .HasOne(u => u.Settings)
                .WithOne()
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Player
            entity
                .HasOne(u => u.Player)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Tournaments (Submitter)
            entity
                .HasMany(u => u.SubmittedTournaments)
                .WithOne(t => t.SubmittedByUser)
                .HasForeignKey(t => t.SubmittedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Tournaments (Verifier)
            // Navigation not mapped
            entity
                .HasMany<Tournament>()
                .WithOne(t => t.VerifiedByUser)
                .HasForeignKey(t => t.VerifiedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Matches (Submitter)
            entity
                .HasMany(u => u.SubmittedMatches)
                .WithOne(m => m.SubmittedByUser)
                .HasForeignKey(m => m.SubmittedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Matches (Verifier)
            entity
                .HasMany<Match>()
                .WithOne(m => m.VerifiedByUser)
                .HasForeignKey(m => m.VerifiedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: OAuthClients
            entity
                .HasMany(u => u.Clients)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Friends
            entity
                .HasMany(u => u.Friends)
                .WithMany(p => p.Followers)
                .UsingEntity<Dictionary<string, object>>(
                    "__join__friends",
                    r => r.HasOne<Player>()
                        .WithMany()
                        .HasForeignKey("player_id")
                        .HasConstraintName("FK___join__friends_player"),
                    l => l.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("user_id")
                        .HasConstraintName("FK___join__friends_user"));

            // == Admin Notes ==
            // Relation: PlayerAdminNotes
            entity
                .HasMany(u => u.PlayerAdminNotes)
                .WithOne(pan => pan.AdminUser)
                .HasForeignKey(pan => pan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: TournamentAdminNotes
            entity
                .HasMany(u => u.TournamentAdminNotes)
                .WithOne(tan => tan.AdminUser)
                .HasForeignKey(tan => tan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: MatchAdminNotes
            entity
                .HasMany(u => u.MatchAdminNotes)
                .WithOne(man => man.AdminUser)
                .HasForeignKey(man => man.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: GameScoreAdminNotes
            entity.HasMany(u => u.GameScoreAdminNotes)
                .WithOne(gsan => gsan.AdminUser)
                .HasForeignKey(gsan => gsan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: GameAdminNotes
            entity
                .HasMany(u => u.GameAdminNotes)
                .WithOne(gan => gan.AdminUser)
                .HasForeignKey(gan => gan.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.Property(us => us.Id).UseIdentityAlwaysColumn();
            entity.Property(us => us.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(us => us.DefaultRuleset).HasDefaultValue(Ruleset.Osu);
            entity.Property(us => us.DefaultRulesetIsControlled).HasDefaultValue(false);

            // Relation: User
            entity
                .HasOne<User>()
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(us => us.UserId).IsUnique();
        });
    }

    public override int SaveChanges()
    {
        SetUpdatedTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetUpdatedTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetUpdatedTimestamps()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: IUpdateableEntity }))
        {
            ((IUpdateableEntity)entry.Entity).Updated = DateTime.UtcNow;
        }
    }
}
