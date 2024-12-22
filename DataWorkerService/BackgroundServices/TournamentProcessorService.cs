using System.Diagnostics;
using Database;
using Database.Entities;
using Database.Enums.Verification;
using Database.Repositories.Interfaces;
using DataWorkerService.Configurations;
using DataWorkerService.Processors;
using DataWorkerService.Processors.Resolvers.Interfaces;
using Microsoft.Extensions.Options;

namespace DataWorkerService.BackgroundServices;

/// <summary>
/// Background service tasked with processing <see cref="Tournament"/>s
/// </summary>
public class TournamentProcessorService(
    ILogger<TournamentProcessorService> logger,
    IServiceProvider serviceProvider,
    IOptions<TournamentProcessingConfiguration> tournamentProcessingConfig
) : ScopeConsumingBackgroundService(logger, serviceProvider)
{
    private readonly TournamentProcessingConfiguration _config = tournamentProcessingConfig.Value;
    private readonly Stopwatch _stopwatch = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.Enabled)
        {
            logger.LogInformation("Background service disabled due to configuration");
            return;
        }

        await base.ExecuteAsync(stoppingToken);
    }

    protected override async Task OnWorkCompleted(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(_config.BatchIntervalSeconds), stoppingToken);
    }

    protected override async Task DoWork(IServiceScope scope, CancellationToken stoppingToken)
    {
        logger.LogInformation("Tournament batch processing started");

        _stopwatch.Start();
        OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();

        ITournamentsRepository tournamentsRepository =
            scope.ServiceProvider.GetRequiredService<ITournamentsRepository>();

        ITournamentProcessorResolver tournamentProcessorResolver =
            scope.ServiceProvider.GetRequiredService<ITournamentProcessorResolver>();

        var tournaments = (await tournamentsRepository.GetNeedingProcessingAsync(_config.BatchSize)).ToList();
        var tasks = new List<Task>();

        foreach (Tournament tournament in tournaments)
        {
            if (tournament.Matches.Count == 0)
            {
                RejectTournamentIncompleteData(tournament);
                continue;
            }

            tasks.Add(ProcessAsync(tournament, tournamentProcessorResolver, stoppingToken));
        }

        await Task.WhenAll(tasks);
        await context.SaveChangesAsync(stoppingToken);

        _stopwatch.Stop();
        logger.LogInformation(
            @"Tournament batch processing completed: [Count: {Count}, Elapsed: {Elapsed:mm\:ss\:fff}]",
            tournaments.Count, _stopwatch.Elapsed);

        _stopwatch.Reset();
    }

    private void RejectTournamentIncompleteData(Tournament tournament)
    {
        tournament.VerificationStatus = VerificationStatus.Rejected;
        tournament.RejectionReason = TournamentRejectionReason.IncompleteData;
        tournament.ProcessingStatus = TournamentProcessingStatus.Done;
        tournament.LastProcessingDate = DateTime.Now;

        logger.LogWarning("Skipping processing for tournament with no matches [Id: {Id}]", tournament.Id);
    }

    private async Task ProcessAsync(Tournament tournament,
        ITournamentProcessorResolver tournamentProcessorResolver, CancellationToken stoppingToken)
    {
        TournamentProcessingStatus processingStatusBefore = tournament.ProcessingStatus;
        logger.LogDebug(
            "Processing starting [Id: {Id} | Processing Status: {Status} | Last Processing: {LastProcessing:MM/dd/yyyy}]",
            tournament.Id,
            tournament.ProcessingStatus,
            tournament.LastProcessingDate
        );

        IProcessor<Tournament> processor = tournamentProcessorResolver.GetNextProcessor(tournament.ProcessingStatus);
        await processor.ProcessAsync(tournament, stoppingToken);

        if (processingStatusBefore != tournament.ProcessingStatus)
        {
            logger.LogInformation(
                "Processing completed [Id: {Id} | Processing Status: {Before} --> {After}]",
                tournament.Id,
                processingStatusBefore,
                tournament.ProcessingStatus
            );
        }
        else
        {
            logger.LogDebug(
                "Processing completed [Id: {Id} | Processing Status: {Before} (No change)]",
                tournament.Id,
                processingStatusBefore
            );
        }
    }
}
