using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;

namespace API.Services.Implementations;

public class BeatmapService : ServiceBase<Beatmap>, IBeatmapService
{
	private readonly ILogger<BeatmapService> _logger;
	public BeatmapService(ICredentials credentials, ILogger<BeatmapService> logger) : base(credentials, logger) { _logger = logger; }

	public async Task<IEnumerable<Beatmap>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_id = ANY(@BeatmapIds)", new { BeatmapIds = beatmapIds });
		}
	}

	public async Task<int> BulkInsertAsync(IEnumerable<Beatmap> beatmaps)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			int i = 0;
			foreach (var beatmap in beatmaps)
			{
				try
				{
					await connection.ExecuteAsync("INSERT INTO beatmaps (artist, beatmap_id, bpm, mapper_id, mapper_name, sr, aim_diff, speed_diff, cs, ar, hp, od, drain_time, length, " +
					                              "title, diff_name, game_mode, circle_count, slider_count, spinner_count, max_combo) VALUES " +
					                              "(@Artist, @BeatmapId, @BPM, @MapperId, @MapperName, @SR, @AimDiff, @SpeedDiff, @CS, @AR, @HP, @OD, @DrainTime, @Length, @Title, @DiffName, " +
					                              "@GameMode, @CircleCount, @SliderCount, @SpinnerCount, @MaxCombo) ON CONFLICT (beatmap_id) DO NOTHING", beatmap);

					i++;
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Failed to insert beatmap {BeatmapId} as part of bulk insert operation", beatmap.BeatmapId);
				}
			}

			return i;
		}
	}

	public async Task<IEnumerable<Beatmap>> GetAllAsync()
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Beatmap>("SELECT * FROM beatmaps WHERE id = 1");
		}
	}

	public async Task<Beatmap?> GetByBeatmapIdAsync(long osuBeatmapId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QuerySingleOrDefaultAsync<Beatmap?>("SELECT * FROM beatmaps WHERE beatmap_id = @Id LIMIT 1", new { Id = osuBeatmapId });
		}
	}
}