using Database.Entities;
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

    public virtual DbSet<BaseStats> BaseStats { get; set; }
    public virtual DbSet<Beatmap> Beatmaps { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameWinRecord> GameWinRecords { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<MatchHistory> MatchHistory { get; set; }
    public virtual DbSet<MatchDuplicate> MatchDuplicates { get; set; }
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
            entity.HasKey(e => e.Id).HasName("BaseStats_pk");
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasOne(d => d.Player)
                .WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("BaseStats___fkplayerid");

            entity.HasIndex(x => new { x.PlayerId, x.Mode }).IsUnique();
            entity.HasIndex(x => x.PlayerId);
            entity.HasIndex(x => x.Rating).IsDescending();
            entity.HasIndex(x => x.Mode);
        });

        modelBuilder.Entity<Beatmap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("beatmaps_pk");
            entity.Property(e => e.Id).UseIdentityColumn();
            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasMany(b => b.Games)
                .WithOne(g => g.Beatmap)
                .HasForeignKey(g => g.BeatmapId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("games_beatmaps_id_fk");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("osugames_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasOne(d => d.Match)
                .WithMany(p => p.Games)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("games_matches_id_fk");

            entity
                .HasOne(g => g.Beatmap)
                .WithMany(b => b.Games)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("games_beatmaps_id_fk")
                .IsRequired(false);

            entity.HasMany(g => g.MatchScores).WithOne(s => s.Game).OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(g => g.WinRecord)
                .WithOne(wr => wr.Game)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("games_game_win_records_id_fk")
                .IsRequired();

            entity.HasIndex(x => x.GameId);
            entity.HasIndex(x => x.MatchId);
            entity.HasIndex(x => x.StartTime);
        });

        modelBuilder.Entity<GameWinRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_win_records_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity
                .HasOne(e => e.Game)
                .WithOne(e => e.WinRecord)
                .HasForeignKey<GameWinRecord>(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("game_win_records_games_id_fk");

            entity.HasIndex(x => x.GameId);
            entity.HasIndex(x => x.Winners);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("matches_pk");
            entity.HasIndex(e => e.MatchId).IsUnique();
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // These are nullable because a user's account could be deleted
            entity.Property(e => e.SubmitterUserId).IsRequired(false).HasDefaultValue(null);
            entity
                .HasOne(e => e.SubmittedBy)
                .WithMany(u => u.SubmittedMatches)
                .HasForeignKey(e => e.SubmitterUserId)
                .IsRequired(false);

            entity
                .HasOne(e => e.VerifiedBy)
                .WithMany(u => u.VerifiedMatches)
                .HasForeignKey(e => e.VerifierUserId)
                .IsRequired(false);
            entity.HasMany(e => e.Games).WithOne(g => g.Match).OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(e => e.Tournament)
                .WithMany(t => t.Matches)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasMany(e => e.Stats)
                .WithOne(s => s.Match)
                .HasForeignKey(e => e.MatchId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.MatchId);
        });

        modelBuilder.Entity<MatchHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("matches_hist_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            // Set nullable and no action on delete because we want to keep the records if a match is deleted
            entity.Property(e => e.ReferenceId).IsRequired(false).HasDefaultValue(null);
            entity
                .HasOne(e => e.ReferenceMatch)
                .WithMany()
                .HasForeignKey(e => e.ReferenceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Set nullable because the API itself performs many edits on matches records
            entity.Property(e => e.ModifierId).IsRequired(false).HasDefaultValue(null);
            // If an edit / delete record is being created, it means the data stopped being available on history creation
            entity.Property(e => e.HistoryEndTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<MatchDuplicate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("match_duplicate_xref_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.HasIndex(e => e.SuspectedDuplicateOf);
            entity.HasIndex(e => e.OsuMatchId);

            entity
                .HasOne(e => e.Verifier)
                .WithMany(e => e.VerifiedDuplicates)
                .HasForeignKey(e => e.VerifiedBy)
                .IsRequired(false);
        });

        modelBuilder.Entity<MatchRatingStats>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("match_rating_stats_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.HasOne(e => e.Player).WithMany(e => e.MatchRatingStats).HasForeignKey(e => e.PlayerId);
            entity.HasOne(e => e.Match).WithMany(e => e.RatingStats).HasForeignKey(e => e.MatchId);
        });

        modelBuilder.Entity<MatchScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("match_scores_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.IsValid).HasDefaultValue(true);

            entity
                .HasOne(d => d.Game)
                .WithMany(p => p.MatchScores)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("match_scores_games_id_fk");

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

            entity
                .HasOne(e => e.Match)
                .WithOne(e => e.WinRecord)
                .HasForeignKey<MatchWinRecord>(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("match_win_records_matches_id_fk");

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

            entity.HasMany(e => e.MatchScores).WithOne(m => m.Player).OnDelete(DeleteBehavior.Cascade);
            entity
                .HasMany(e => e.RatingAdjustments)
                .WithOne(ra => ra.Player)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Ratings).WithOne(r => r.Player).OnDelete(DeleteBehavior.Cascade);
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

            entity.HasOne(e => e.Player).WithMany(e => e.MatchStats).HasForeignKey(e => e.PlayerId);
            entity.HasOne(e => e.Match).WithMany(e => e.Stats).HasForeignKey(e => e.MatchId);

            entity.HasIndex(e => e.PlayerId);
            entity.HasIndex(e => new { e.PlayerId, e.MatchId }).IsUnique();
            entity.HasIndex(e => new { e.PlayerId, e.Won });
        });

        modelBuilder.Entity<RatingAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RatingAdjustment_pk");
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.HasOne(e => e.Player).WithMany(e => e.RatingAdjustments).HasForeignKey(e => e.PlayerId);

            entity.HasIndex(e => new { e.PlayerId, e.Mode });
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

            entity.HasMany(e => e.Clients).WithOne(e => e.User).IsRequired(false);

            entity
                .HasOne(d => d.Player)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("Users___fkplayerid")
                .IsRequired(false);

            entity
                .HasMany(e => e.VerifiedDuplicates)
                .WithOne(e => e.Verifier)
                .HasForeignKey(e => e.VerifiedBy)
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
