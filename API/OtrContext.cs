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

	public virtual DbSet<Beatmap> Beatmaps { get; set; }
	public virtual DbSet<BeatmapModSr> BeatmapModSrs { get; set; }
	public virtual DbSet<Config> Configs { get; set; }
	public virtual DbSet<Game> Games { get; set; }
	public virtual DbSet<Match> Matches { get; set; }
	public virtual DbSet<MatchScore> MatchScores { get; set; }
	public virtual DbSet<Player> Players { get; set; }
	public virtual DbSet<Rating> Ratings { get; set; }
	public virtual DbSet<RatingHistory> RatingHistories { get; set; }
	public virtual DbSet<User> Users { get; set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
		optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Beatmap>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("beatmaps_pk");
			entity.Property(e => e.Id).UseIdentityColumn();
			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasMany(b => b.Games)
			      .WithOne(g => g.Beatmap)
			      .HasForeignKey(g => g.BeatmapId)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("games_beatmaps_id_fk");

			entity.HasMany(e => e.BeatmapModSrs);
			entity.Navigation(e => e.BeatmapModSrs).AutoInclude();
		});

		modelBuilder.Entity<BeatmapModSr>(entity =>
		{
			entity
				.HasKey(b => new { b.BeatmapId, b.Mods });

			entity
				.HasIndex(b => b.BeatmapId)
				.IsUnique(false);
		});

		modelBuilder.Entity<Config>(entity => { entity.Property(e => e.Id).ValueGeneratedOnAdd(); });

		modelBuilder.Entity<Game>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("osugames_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasOne(d => d.Match)
			      .WithMany(p => p.Games)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("games_matches_id_fk");

			entity.HasOne(g => g.Beatmap)
			      .WithMany(b => b.Games)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("games_beatmaps_id_fk")
			      .IsRequired(false);

			entity.HasMany(g => g.MatchScores)
			      .WithOne(s => s.Game)
			      .OnDelete(DeleteBehavior.ClientSetNull);

			entity.HasIndex(x => x.GameId);
			entity.HasIndex(x => x.MatchId);
			entity.HasIndex(x => x.TeamType);
			entity.HasIndex(x => new { x.TeamType, x.Id });
			entity.HasIndex(x => x.StartTime);
			entity.HasIndex(x => x.PlayMode);
		});

		modelBuilder.Entity<Match>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("matches_pk");
			entity.HasIndex(e => e.MatchId).IsUnique();
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
			
			entity.Property(e => e.SubmitterUserId).IsRequired(false).HasDefaultValue(null);
			entity.HasOne(e => e.SubmittedBy).WithMany(u => u.SubmittedMatches).HasForeignKey(e => e.SubmitterUserId).IsRequired(false);
			entity.HasMany(e => e.Games).WithOne(g => g.Match);
			entity.HasMany(e => e.RatingHistories).WithOne(h => h.Match);

			entity.HasIndex(x => x.VerificationStatus);
			entity.HasIndex(x => x.StartTime);
			entity.HasIndex(x => x.MatchId);
		});

		modelBuilder.Entity<MatchScore>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("match_scores_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.HasOne(d => d.Game)
			      .WithMany(p => p.MatchScores)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("match_scores_games_id_fk");

			entity.HasOne(d => d.Player)
			      .WithMany(p => p.MatchScores)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("match_scores_players_id_fk");

			entity.HasIndex(x => x.PlayerId);
			entity.HasIndex(x => x.GameId);
			entity.HasIndex(x => new { x.GameId, x.Team });
			entity.HasIndex(x => x.Team);
			entity.HasIndex(x => x.EnabledMods);
		});

		modelBuilder.Entity<Player>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Player_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasMany(e => e.MatchScores).WithOne(m => m.Player);
			entity.HasMany(e => e.RatingHistories).WithOne(r => r.Player);
			entity.HasMany(e => e.Ratings).WithOne(r => r.Player);
			entity.HasOne(e => e.User).WithOne(u => u.Player);

			entity.HasIndex(x => x.OsuId);
			entity.HasIndex(x => x.Country);
			entity.HasIndex(x => x.Username);
		});

		modelBuilder.Entity<Rating>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("Ratings_pk");
			entity.Property(e => e.Id).UseIdentityAlwaysColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasOne(d => d.Player)
			      .WithMany(p => p.Ratings)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("Ratings___fkplayerid");

			entity.HasIndex(x => x.PlayerId);
			entity.HasIndex(x => x.Mode);
			entity.HasIndex(x => x.Mu);
			entity.HasIndex(x => new { x.PlayerId, x.Mode });
		});

		modelBuilder.Entity<RatingHistory>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("RatingHistories_pk");
			entity.Property(e => e.Id).UseIdentityAlwaysColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");

			entity.HasOne(d => d.Match)
			      .WithMany(p => p.RatingHistories)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("ratinghistories_matches_id_fk");

			entity.HasOne(d => d.Player)
			      .WithMany(p => p.RatingHistories)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("RatingHistories___fkplayerid");

			entity.HasIndex(x => x.Mode);
			entity.HasIndex(x => x.Mu);
			entity.HasIndex(x => x.Created);
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("User_pk");
			entity.Property(e => e.Id).UseIdentityColumn();

			entity.Property(e => e.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
			entity.Property(e => e.Roles);

			entity.HasOne(d => d.Player)
			      .WithOne(p => p.User)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("Users___fkplayerid")
			      .IsRequired(false);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}