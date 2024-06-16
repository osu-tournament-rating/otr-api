using Database.Entities;
using Database.Entities.Processor;
using Database.Enums;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Database;

public class OtrContext(
    DbContextOptions<OtrContext> options) : DbContext(options)
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

    public virtual DbSet<BaseStats> BaseStats { get; set; }
    public virtual DbSet<Beatmap> Beatmaps { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameWinRecord> GameWinRecords { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchRatingStats> MatchRatingStats { get; set; }
    public virtual DbSet<MatchScore> MatchScores { get; set; }
    public virtual DbSet<MatchWinRecord> MatchWinRecords { get; set; }
    public virtual DbSet<OAuthClient> OAuthClients { get; set; }
    public virtual DbSet<Player> Players { get; set; }
    public virtual DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
    public virtual DbSet<RatingAdjustment> RatingAdjustments { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseStats>(entity =>
        {
            entity.Property(bs => bs.Id).UseIdentityAlwaysColumn();

            entity.Property(bs => bs.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(bs => bs.Player)
                .WithMany(p => p.Ratings)
                .HasForeignKey(bs => bs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Mode);
            entity.HasIndex(x => x.PlayerId);
            entity.HasIndex(x => x.Rating).IsDescending();
            entity.HasIndex(x => new { x.PlayerId, x.Mode }).IsUnique();
        });

        modelBuilder.Entity<Beatmap>(entity =>
        {
            entity.Property(b => b.Id).UseIdentityAlwaysColumn();

            entity.Property(b => b.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Games
            entity
                .HasMany(b => b.Games)
                .WithOne(g => g.Beatmap)
                .HasForeignKey(g => g.BeatmapId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(b => b.BeatmapId).IsUnique();
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

            // Relation: MatchScores
            entity
                .HasMany(g => g.MatchScores)
                .WithOne(ms => ms.Game)
                .HasForeignKey(ms => ms.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(g => g.MatchId);
            entity.HasIndex(g => g.StartTime);
            entity.HasIndex(g => g.GameId).IsUnique();
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

            entity.HasIndex(x => x.Winners);
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

            // Relation: MatchRatingStats
            entity
                .HasMany(m => m.MatchRatingStats)
                .WithOne(mrs => mrs.Match)
                .HasForeignKey(mrs => mrs.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: PlayerMatchStats
            entity
                .HasMany(m => m.PlayerMatchStats)
                .WithOne(pms => pms.Match)
                .HasForeignKey(pms => pms.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => m.MatchId).IsUnique();
        });

        modelBuilder.Entity<MatchRatingStats>(entity =>
        {
            entity.Property(mrs => mrs.Id).UseIdentityAlwaysColumn();

            entity.Property(m => m.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(mrs => mrs.Player)
                .WithMany(p => p.MatchRatingStats)
                .HasForeignKey(mrs => mrs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Match
            entity
                .HasOne(mrs => mrs.Match)
                .WithMany(m => m.MatchRatingStats)
                .HasForeignKey(mrs => mrs.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(mrs => new { mrs.PlayerId, mrs.MatchId }).IsUnique();
        });

        modelBuilder.Entity<MatchScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("match_scores_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.IsValid).HasDefaultValue(true);

            // Relation: Game
            entity
                .HasOne(ms => ms.Game)
                .WithMany(g => g.MatchScores)
                .HasForeignKey(ms => ms.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(d => d.Player)
                .WithMany(p => p.MatchScores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("match_scores_players_id_fk");

            entity.HasIndex(x => x.PlayerId);
        });

        modelBuilder.Entity<MatchWinRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("match_win_records_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            // Relation: Match
            entity
                .HasOne(mwr => mwr.Match)
                .WithOne(m => m.WinRecord)
                .HasForeignKey<MatchWinRecord>(mwr => mwr.MatchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.LoserRoster);
            entity.HasIndex(e => e.WinnerRoster);
        });

        modelBuilder.Entity<OAuthClient>(entity =>
        {
            entity.HasKey(x => x.Id).HasName("oauth_clients_pk");
            entity.Property(x => x.Id).UseIdentityColumn();

            entity
                .OwnsOne(e => e.RateLimitOverrides, rlo =>
                {
                    rlo.ToJson("rate_limit_overrides");
                    rlo.Property(p => p.PermitLimit).HasDefaultValue(null);
                    rlo.Property(p => p.Window).HasDefaultValue(null);
                });

            entity.Property(x => x.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasOne(e => e.User)
                .WithMany(e => e.Clients)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Player_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasMany(e => e.MatchScores)
                .WithOne(m => m.Player)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: RatingAdjustments
            entity
                .HasMany(e => e.RatingAdjustments)
                .WithOne(ra => ra.Player)
                .HasForeignKey(ra => ra.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: BaseStats
            entity
                .HasMany(p => p.Ratings)
                .WithOne(bs => bs.Player)
                .HasForeignKey(bs => bs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: MatchRatingStats
            entity
                .HasMany(p => p.MatchRatingStats)
                .WithOne(mrs => mrs.Player)
                .HasForeignKey(mrs => mrs.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(e => e.User)
                .WithOne(u => u.Player)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.OsuId).IsUnique();
        });

        modelBuilder.Entity<PlayerMatchStats>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PlayerMatchStats_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity
                .HasOne(e => e.Player)
                .WithMany(e => e.MatchStats)
                .HasForeignKey(e => e.PlayerId);

            entity
                .HasOne(pms => pms.Match)
                .WithMany(m => m.PlayerMatchStats)
                .HasForeignKey(pms => pms.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PlayerId);
            entity.HasIndex(e => new { e.PlayerId, e.MatchId }).IsUnique();
            entity.HasIndex(e => new { e.PlayerId, e.Won });
        });

        modelBuilder.Entity<RatingAdjustment>(entity =>
        {
            entity.Property(ra => ra.Id).UseIdentityAlwaysColumn();

            entity.Property(ra => ra.Created).HasDefaultValueSql(SqlCurrentTimestamp);

            // Relation: Player
            entity
                .HasOne(ra => ra.Player)
                .WithMany(p => p.RatingAdjustments)
                .HasForeignKey(ra => ra.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PlayerId, e.Ruleset });
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
            entity.HasKey(e => e.Id).HasName("User_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relation: UserSettings
            entity
                .HasOne(u => u.Settings)
                .WithOne()
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .OwnsOne(e => e.RateLimitOverrides, rlo =>
                {
                    rlo.ToJson("rate_limit_overrides");
                    rlo.Property(p => p.PermitLimit).HasDefaultValue(null);
                    rlo.Property(p => p.Window).HasDefaultValue(null);
                });

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

            entity.HasMany(e => e.Clients).WithOne(e => e.User).IsRequired(false);

            entity
                .HasOne(d => d.Player)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("Users___fkplayerid")
                .IsRequired(false);
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
