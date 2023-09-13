using API.Entities;
using API.Osu.Multiplayer;
using API.Services.Interfaces;

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
				var beatmapsService = scope.ServiceProvider.GetRequiredService<IBeatmapService>();
				await ProcessBeatmapModSrsAsync(beatmapsService);
				await Task.Delay(30 * 1000, stoppingToken);
			}
		}
	}

	private async Task ProcessBeatmapModSrsAsync(IBeatmapService beatmapService)
	{
		var maps = await beatmapService.GetUnprocessedSrBeatmapIdsAsync();

		var semaphore = new SemaphoreSlim(4);
    
		var batchSize = 100;
		for (int i = 0; i < maps.Count; i += batchSize)
		{
			var currentBatch = maps.Skip(i).Take(batchSize).ToList();
			var tasks = currentBatch.Select(map => ProcessMapAsync(map, semaphore, beatmapService)).ToList();

			// Wait for the current batch to complete before processing the next one.
			await Task.WhenAll(tasks);
		}
	}

	private async Task ProcessMapAsync((int mapId, OsuEnums.Mods mods) map, SemaphoreSlim semaphore, IBeatmapService beatmapService)
	{
		await semaphore.WaitAsync(); // Acquire semaphore

		try
		{
			int mapId = map.mapId;
			OsuEnums.Mods mods = map.mods;
			long osuMapId = await beatmapService.GetBeatmapIdAsync(mapId);
			var beatmap = await _osuApiService.GetBeatmapAsync(osuMapId, mods);

			if (beatmap == null)
			{
				_logger.LogWarning("Failed to fetch beatmap while processing mod sr {BeatmapId} (result from API was null)", osuMapId);
				return;
			}

			await beatmapService.InsertModSrAsync(new BeatmapModSr
			{
				BeatmapId = mapId,
				Mods = (int)mods,
				PostModSr = beatmap.Sr
			});

			_logger.LogDebug("Inserted mod sr for {BeatmapId} with mods {Mods}", osuMapId, mods);
		}
		finally
		{
			semaphore.Release(); // Release semaphore
		}
	}

}