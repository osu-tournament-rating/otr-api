using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Database;

public class OtrContext(
    DbContextOptions<OtrContext> options) : DbContext(options)
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
    public virtual DbSet<DataWorkerQueueMatch> DataWorkerQueueMatches { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameWinRecord> GameWinRecords { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchAudit> MatchAudits { get; set; }
    public virtual DbSet<RatingAdjustment> RatingAdjustments { get; set; }
    public virtual DbSet<GameScore> GameScores { get; set; }
    public virtual DbSet<MatchWinRecord> MatchWinRecords { get; set; }
    public virtual DbSet<OAuthClient> OAuthClients { get; set; }
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
    public virtual DbSet<PlayerRating> PlayerRatings { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(_auditingInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var changesConverter = new ValueConverter<IDictionary<string, AuditChangelogEntry>, string>(
            v => JsonConvert.SerializeObject(v, Formatting.None),
            v => JsonConvert.DeserializeObject<IDictionary<string, AuditChangelogEntry>>(v)
                 ?? new Dictionary<string, AuditChangelogEntry>()
        );

        var changesComparer = new ValueComparer<IDictionary<string, AuditChangelogEntry>>(
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

        modelBuilder.Entity<DataWorkerQueueMatch>(entity =>
        {
            entity.Property(qm => qm.Id).UseIdentityAlwaysColumn();

            entity.Property(qm => qm.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.HasIndex(qm => qm.OsuMatchId).IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(g => g.Id).UseIdentityAlwaysColumn();

            entity.Property(g => g.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(g => g.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(g => g.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

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

            entity.HasIndex(g => g.MatchId);
            entity.HasIndex(g => g.StartTime);
            entity.HasIndex(g => g.OsuId).IsUnique();
        });

        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.Property(ms => ms.Id).UseIdentityAlwaysColumn();

            entity.Property(ms => ms.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Game
            entity
                .HasOne(ms => ms.Game)
                .WithMany(g => g.Scores)
                .HasForeignKey(ms => ms.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Player
            entity
                .HasOne(ms => ms.Player)
                .WithMany(p => p.Scores)
                .HasForeignKey(ms => ms.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ms => ms.PlayerId);
            entity.HasIndex(ms => new { ms.PlayerId, ms.GameId }).IsUnique();
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

            entity.Property(m => m.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(m => m.StartTime).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(m => m.EndTime).HasDefaultValueSql(SqlPlaceholderDate);

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
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => m.OsuId).IsUnique();
        });

        modelBuilder.Entity<MatchAudit>(entity =>
        {
            entity.Property(ma => ma.Id).UseIdentityAlwaysColumn();

            entity.Property(ma => ma.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(ma => ma.Changes)
                .HasConversion(changesConverter)
                .Metadata.SetValueComparer(changesComparer);

            // Relation: Match
            entity
                .HasOne<Match>()
                .WithMany(m => m.Audits)
                .HasForeignKey(ma => ma.ReferenceId)
                .OnDelete(DeleteBehavior.Restrict);
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

            // Relation: User
            entity
                .HasOne(c => c.User)
                .WithMany(u => u.Clients)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(p => p.Id).UseIdentityAlwaysColumn();

            entity.Property(p => p.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(p => p.OsuLastFetch).HasDefaultValueSql(SqlPlaceholderDate);
            entity.Property(p => p.OsuTrackLastFetch).HasDefaultValueSql(SqlPlaceholderDate);

            entity.OwnsMany(p => p.RulesetData,
                rd => rd.ToJson("ruleset_data"));

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

            entity.Property(t => t.Created).HasDefaultValueSql(SqlCurrentTimestamp);

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

            entity.HasIndex(t => t.Ruleset);
            entity.HasIndex(t => new { t.Name, t.Abbreviation }).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Id).UseIdentityColumn();

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
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.Property(us => us.Id).UseIdentityAlwaysColumn();

            entity.Property(us => us.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            entity.Property(us => us.DefaultRuleset).HasDefaultValue(Ruleset.Standard);

            // Relation: User
            entity
                .HasOne<User>()
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(us => us.UserId).IsUnique();
        });
    }
}
