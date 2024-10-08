using Database.Entities;
using Database.Entities.Interfaces;
using Database.Entities.Processor;
using Database.Enums;
using Database.Enums.Verification;
using Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Database;

public class OtrContext(DbContextOptions<OtrContext> options) : DbContext(options)
{
    private readonly AuditingInterceptor _auditingInterceptor = new();

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
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameAudit> GameAudits { get; set; }
    public virtual DbSet<GameScore> GameScores { get; set; }
    public virtual DbSet<GameScoreAudit> GameScoreAudits { get; set; }
    public virtual DbSet<GameWinRecord> GameWinRecords { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchAdminNote> MatchAdminNotes { get; set; }
    public virtual DbSet<MatchAudit> MatchAudits { get; set; }
    public virtual DbSet<MatchWinRecord> MatchWinRecords { get; set; }
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(_auditingInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var auditChangesConverter = new ValueConverter<IDictionary<string, AuditChangelogEntry>, string>(
            v => JsonConvert.SerializeObject(v, Formatting.None),
            v => JsonConvert.DeserializeObject<IDictionary<string, AuditChangelogEntry>>(v)
                 ?? new Dictionary<string, AuditChangelogEntry>()
        );

        var auditChangesComparer = new ValueComparer<IDictionary<string, AuditChangelogEntry>>(
            (c1, c2) => JsonConvert.SerializeObject(c1) == JsonConvert.SerializeObject(c2),
            c => JsonConvert.SerializeObject(c).GetHashCode(),
            c => c.ToDictionary(
                e => e.Key,
                e => (AuditChangelogEntry)e.Value.Clone()
            )
        );

        modelBuilder.Entity<Beatmap>(entity =>
        {
            entity.Property(b => b.Id).UseIdentityAlwaysColumn();

            entity.Property(b => b.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(b => b.HasData).HasDefaultValue(true);

            // Relation: Games
            entity
                .HasMany(b => b.Games)
                .WithOne(g => g.Beatmap)
                .HasForeignKey(g => g.BeatmapId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(b => b.OsuId).IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(g => g.Id).UseIdentityAlwaysColumn();

            entity.Property(g => g.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(g => g.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(g => g.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

            entity.Property(g => g.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

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
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: GameWinRecord
            entity
                .HasOne(g => g.WinRecord)
                .WithOne(gwr => gwr.Game)
                .HasForeignKey<GameWinRecord>(gwr => gwr.GameId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: GameScore
            entity
                .HasMany(g => g.Scores)
                .WithOne(gs => gs.Game)
                .HasForeignKey(gs => gs.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Audits
            entity
                .HasMany(g => g.Audits)
                .WithOne()
                .HasForeignKey(ga => ga.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relation: Admin Notes
            entity
                .HasMany(g => g.AdminNotes)
                .WithOne(an => an.Game)
                .HasForeignKey(an => an.ReferenceId)
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

            entity.Property(ga => ga.Changes)
                .HasConversion(auditChangesConverter)
                .Metadata.SetValueComparer(auditChangesComparer);

            // Relation: Game
            entity
                .HasOne<Game>()
                .WithMany(g => g.Audits)
                .HasForeignKey(ga => ga.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.Property(gs => gs.Id).UseIdentityAlwaysColumn();

            entity.Property(gs => gs.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(gs => gs.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

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

            // Relation: Audits
            entity
                .HasMany(gs => gs.Audits)
                .WithOne()
                .HasForeignKey(gsa => gsa.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);

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

            entity.Property(gsa => gsa.Changes)
                .HasConversion(auditChangesConverter)
                .Metadata.SetValueComparer(auditChangesComparer);

            // Relation: GameScore
            entity
                .HasOne<GameScore>()
                .WithMany(gs => gs.Audits)
                .HasForeignKey(gsa => gsa.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<GameWinRecord>(entity =>
        {
            entity.Property(gwr => gwr.Id).UseIdentityAlwaysColumn();

            entity.Property(gwr => gwr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Game
            entity
                .HasOne(gwr => gwr.Game)
                .WithOne(g => g.WinRecord)
                .HasForeignKey<GameWinRecord>(gwr => gwr.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.WinnerRoster);
            entity.HasIndex(x => x.GameId).IsUnique();
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(m => m.Id).UseIdentityAlwaysColumn();

            entity.Property(m => m.Name).HasDefaultValue(string.Empty);
            entity.Property(m => m.VerificationStatus).HasDefaultValue(VerificationStatus.None);
            entity.Property(m => m.RejectionReason).HasDefaultValue(MatchRejectionReason.None);
            entity.Property(m => m.ProcessingStatus).HasDefaultValue(MatchProcessingStatus.NeedsData);

            entity.Property(m => m.Created).HasDefaultValueSql(SqlCurrentTimestamp);
            entity.Property(m => m.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(m => m.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

            entity.Property(m => m.LastProcessingDate).HasDefaultValueSql(SqlPlaceholderDate);

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

            // Relation: MatchWinRecord
            entity
                .HasOne(m => m.WinRecord)
                .WithOne(mwr => mwr.Match)
                .HasForeignKey<MatchWinRecord>(mwr => mwr.MatchId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Relation: Audits
            entity
                .HasMany(m => m.Audits)
                .WithOne()
                .HasForeignKey(a => a.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);

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

            entity.Property(ma => ma.Changes)
                .HasConversion(auditChangesConverter)
                .Metadata.SetValueComparer(auditChangesComparer);

            // Relation: Match
            entity
                .HasOne<Match>()
                .WithMany(m => m.Audits)
                .HasForeignKey(ma => ma.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MatchWinRecord>(entity =>
        {
            entity.Property(mwr => mwr.Id).UseIdentityAlwaysColumn();

            entity.Property(mwr => mwr.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Match
            entity
                .HasOne(mwr => mwr.Match)
                .WithOne(m => m.WinRecord)
                .HasForeignKey<MatchWinRecord>(mwr => mwr.MatchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(mwr => mwr.LoserRoster);
            entity.HasIndex(mwr => mwr.WinnerRoster);
            entity.HasIndex(mwr => mwr.MatchId).IsUnique();
        });

        modelBuilder.Entity<OAuthClient>(entity =>
        {
            entity.Property(c => c.Id).UseIdentityAlwaysColumn();

            entity.Property(c => c.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // RateLimitOverrides as an object is stored in a column as JSON
            entity
                .OwnsOne(e => e.RateLimitOverrides, rlo =>
                {
                    rlo.ToJson("rate_limit_overrides");
                    rlo.Property(p => p.PermitLimit).HasDefaultValue(null);
                    rlo.Property(p => p.Window).HasDefaultValue(null);
                });

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
            entity.Property(p => p.Ruleset).HasDefaultValue(Ruleset.Osu);
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
            entity.HasIndex(pr => pr.Rating).IsDescending(true);
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

            entity.Property(t => t.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(t => t.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

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

            // Relation: Audits
            entity
                .HasMany(t => t.Audits)
                .WithOne()
                .HasForeignKey(ta => ta.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);

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
                .UsingEntity<Dictionary<string, object>>(
                    "__join__pooled_beatmaps",
                    r => r.HasOne<Beatmap>()
                        .WithMany()
                        .HasForeignKey("beatmap_id")
                        .HasConstraintName("FK_JoinTable_Beatmap"),
                    l => l.HasOne<Tournament>()
                        .WithMany()
                        .HasForeignKey("tournament_id")
                        .HasConstraintName("FK_JoinTable_Tournament"));

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

            entity.Property(ta => ta.Changes)
                .HasConversion(auditChangesConverter)
                .Metadata.SetValueComparer(auditChangesComparer);

            // Relation: Game
            entity
                .HasOne<Tournament>()
                .WithMany(t => t.Audits)
                .HasForeignKey(ta => ta.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Id).UseIdentityColumn();
            entity.Property(u => u.Scopes).HasDefaultValue(Array.Empty<string>());
            entity.Property(u => u.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // RateLimitOverrides as an object is stored in a column as JSON
            entity
                .OwnsOne(e => e.RateLimitOverrides, rlo =>
                {
                    rlo.ToJson("rate_limit_overrides");
                    rlo.Property(p => p.PermitLimit).HasDefaultValue(null);
                    rlo.Property(p => p.Window).HasDefaultValue(null);
                });

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
            // Navigation not mapped
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
