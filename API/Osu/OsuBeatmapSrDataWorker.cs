using API.Entities;
using API.Osu.Multiplayer;
using API.Repositories.Interfaces;

namespace API.Osu;

public class OsuBeatmapSrDataWorker : BackgroundService
{
	private readonly ILogger<OsuBeatmapSrDataWorker> _logger;
	private readonly IOsuApiService _osuApiService;
	private readonly IServiceProvider _serviceProvider;

	public OsuBeatmapSrDataWorker(ILogger<OsuBeatmapSrDataWorker> logger, IOsuApiService osuApiService, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_osuApiService = osuApiService;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Initialized osu! beatmap SR data worker");

		await BackgroundTask(stoppingToken);
	}

	private async Task BackgroundTask(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var beatmapsService = scope.ServiceProvider.GetRequiredService<IBeatmapRepository>();
				await ProcessBeatmapModSrsAsync(beatmapsService);
				await Task.Delay(30 * 1000, stoppingToken);
			}
		}
	}

	private async Task ProcessBeatmapModSrsAsync(IBeatmapRepository beatmapRepository)
	{
		var maps = await beatmapRepository.GetUnprocessedSrBeatmapIdsAsync();
		var toAdd = new List<BeatmapModSr>();
		foreach ((int mapId, OsuEnums.Mods mods) in maps)
		{
			if(toAdd.Count == 1000)
			{
				await beatmapRepository.BulkInsertAsync(toAdd);
				toAdd.Clear();
			}
			
			long osuMapId = await beatmapRepository.GetBeatmapIdAsync(mapId);
			var beatmap = await _osuApiService.GetBeatmapAsync(osuMapId, "osu! beatmap SR data worker identified maps that need to be processed", mods);
			
			if (beatmap == null)
			{
				/**
				 * Insert with defaults -- this is extremely rare but does happen
				 * 
				 * Case:
				 * We have already processed the beatmap data before,
				 * but between now and then, the beatmap has been deleted from osu
				 */
				var storedMap = await beatmapRepository.GetByOsuIdAsync(mapId);

				if (storedMap == null)
				{
					_logger.LogError("A beatmap with id {Id} scheduled for processing returned null! Was it deleted from the database?", mapId);
					continue;
				}

				var insert = new BeatmapModSr
				{
					BeatmapId = mapId,
					Mods = (int)mods,
					PostModSr = storedMap.Sr
				};
				
				toAdd.Add(insert);
				
				_logger.LogWarning("Failed to fetch beatmap while processing mod sr (result from API was null), " +
				                   "cached default nomod SR for map: {@BeatmapSr}", insert);
				continue;
			}
			
			toAdd.Add(new BeatmapModSr
			{
				BeatmapId = mapId,
				Mods = (int)mods,
				PostModSr = beatmap.Sr
			});
		}

		if (toAdd.Any())
		{
			await beatmapRepository.BulkInsertAsync(toAdd);
		}
	}
}