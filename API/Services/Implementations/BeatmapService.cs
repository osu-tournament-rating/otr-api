using API.Configurations;
using API.Entities;
using API.Services.Interfaces;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace API.Services.Implementations;

public class BeatmapService : ServiceBase<Beatmap>, IBeatmapService
{
	public BeatmapService(ICredentials credentials, ILogger<BeatmapService> logger) : base(credentials, logger) {}

	public async Task<IEnumerable<Beatmap>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			return await connection.QueryAsync<Beatmap>("SELECT * FROM beatmaps WHERE beatmap_id = ANY(@BeatmapIds)", new { BeatmapIds = beatmapIds });
		}
	}

	public async Task<ulong> BulkInsertAsync(IEnumerable<Beatmap> beatmaps)
	{
		using (var connection = new NpgsqlConnection(ConnectionString))
		{
			await connection.OpenAsync();

			using (var writer = connection.BeginBinaryImport(
				       "COPY beatmaps (artist, beatmap_id, bpm, mapper_id, mapper_name, sr, aim_diff, speed_diff, " +
				       "cs, ar, hp, od, drain_time, total_length, title, diff_name, " +
				       "game_mode, circle_count, slider_count, spinner_count, max_combo) FROM STDIN (FORMAT BINARY)"))
			{
				foreach (var beatmap in beatmaps)
				{
					await writer.StartRowAsync();

					await writer.WriteAsync(beatmap.Artist, NpgsqlDbType.Text);
					await writer.WriteAsync(beatmap.BeatmapId, NpgsqlDbType.Bigint);
					await writer.WriteAsync(beatmap.BPM, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.MapperId, NpgsqlDbType.Bigint);
					await writer.WriteAsync(beatmap.MapperName, NpgsqlDbType.Text);
					await writer.WriteAsync(beatmap.SR, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.AimDiff, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.SpeedDiff, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.CS, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.AR, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.HP, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.OD, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.DrainTime, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.Length, NpgsqlDbType.Double);
					await writer.WriteAsync(beatmap.Title, NpgsqlDbType.Text);
					await writer.WriteAsync(beatmap.DiffName, NpgsqlDbType.Text);
					await writer.WriteAsync((int) beatmap.GameMode, NpgsqlDbType.Integer);
					await writer.WriteAsync(beatmap.CircleCount, NpgsqlDbType.Integer);
					await writer.WriteAsync(beatmap.SliderCount, NpgsqlDbType.Integer);
					await writer.WriteAsync(beatmap.SpinnerCount, NpgsqlDbType.Integer);
					await writer.WriteAsync(beatmap.MaxCombo, NpgsqlDbType.Integer);
				}

				return await writer.CompleteAsync();
			}
		}
	}
}