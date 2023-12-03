using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public partial class OtrContext : DbContext
{
	private readonly IConfiguration _configuration;
	public OtrContext() {}

	public OtrContext(DbContextOptions<OtrContext> options, IConfiguration configuration)
		: base(options)
	{
		_configuration = configuration;
	}

	public virtual DbSet<BaseStats> BaseStats { get; set; }
	public virtual DbSet<Beatmap> Beatmaps { get; set; }
	public virtual DbSet<Config> Configs { get; set; }
	public virtual DbSet<Game> Games { get; set; }
	public virtual DbSet<Match> Matches { get; set; }
	public virtual DbSet<MatchRatingStats> MatchRatingStats { get; set; }
	public virtual DbSet<MatchScore> MatchScores { get; set; }
	public virtual DbSet<Player> Players { get; set; }
	public virtual DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
	public virtual DbSet<RatingAdjustment> RatingAdjustments { get; set; }
	public virtual DbSet<Tournament> Tournaments { get; set; }
	public virtual DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
		optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<BaseStats>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("BaseStats_pk");
			entity.Property(e => e.Id).UseIdentityAlwaysColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasOne(d => d.Player)
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

			entity.HasMany(b => b.Games)
			      .WithOne(g => g.Beatmap)
			      .HasForeignKey(g => g.BeatmapId)
			      .OnDelete(DeleteBehavior.NoAction)
			      .HasConstraintName("games_beatmaps_id_fk");
		});

		modelBuilder.Entity<Config>(entity => { entity.Property(e => e.Id).ValueGeneratedOnAdd(); });

		modelBuilder.Entity<Game>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("osugames_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasOne(d => d.Match)
			      .WithMany(p => p.Games)
			      .OnDelete(DeleteBehavior.NoAction)
			      .HasConstraintName("games_matches_id_fk");

			entity.HasOne(g => g.Beatmap)
			      .WithMany(b => b.Games)
			      .OnDelete(DeleteBehavior.NoAction)
			      .HasConstraintName("games_beatmaps_id_fk")
			      .IsRequired(false);

			entity.HasMany(g => g.MatchScores)
			      .WithOne(s => s.Game)
			      .OnDelete(DeleteBehavior.Cascade);

			entity.HasIndex(x => x.GameId);
			entity.HasIndex(x => x.MatchId);
			entity.HasIndex(x => x.StartTime);
		});

		modelBuilder.Entity<Match>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("matches_pk");
			entity.HasIndex(e => e.MatchId).IsUnique();
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
			
			entity.Property(e => e.SubmitterUserId).IsRequired(false).HasDefaultValue(null);
			entity.HasOne(e => e.SubmittedBy).WithMany(u => u.SubmittedMatches).HasForeignKey(e => e.SubmitterUserId).IsRequired(false);
			entity.HasOne(e => e.VerifiedBy).WithMany(u => u.VerifiedMatches).HasForeignKey(e => e.VerifierUserId).IsRequired(false);
			entity.HasMany(e => e.Games).WithOne(g => g.Match).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(e => e.Tournament).WithMany(t => t.Matches).IsRequired(false);
			entity.HasMany(e => e.Stats).WithOne(s => s.Match).HasForeignKey(e => e.MatchId).IsRequired().OnDelete(DeleteBehavior.Cascade);

			entity.HasIndex(x => x.MatchId);
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

			entity.HasOne(d => d.Game)
			      .WithMany(p => p.MatchScores)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("match_scores_games_id_fk");

			entity.HasOne(d => d.Player)
			      .WithMany(p => p.MatchScores)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("match_scores_players_id_fk");

			entity.HasIndex(x => x.PlayerId);
		});

		modelBuilder.Entity<Player>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Player_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasMany(e => e.MatchScores).WithOne(m => m.Player).OnDelete(DeleteBehavior.Cascade);
			entity.HasMany(e => e.RatingAdjustments).WithOne(ra => ra.Player).OnDelete(DeleteBehavior.Cascade);
			entity.HasMany(e => e.Ratings).WithOne(r => r.Player).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(e => e.User).WithOne(u => u.Player).IsRequired(false).OnDelete(DeleteBehavior.Cascade);

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
			entity.HasKey(e => e.Id).HasName("Tournaments_pk");
			
			entity.Property(e => e.Id).UseIdentityAlwaysColumn();
			
			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasMany(e => e.Matches)
			      .WithOne(m => m.Tournament)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("Tournaments___fkmatchid")
			      .IsRequired(false);
			
			entity.HasIndex(e => new { e.Name, e.Abbreviation }).IsUnique();
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("User_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.Roles);

			entity.HasOne(d => d.Player)
			      .WithOne(p => p.User)
			      .OnDelete(DeleteBehavior.NoAction)
			      .HasConstraintName("Users___fkplayerid")
			      .IsRequired(false);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}