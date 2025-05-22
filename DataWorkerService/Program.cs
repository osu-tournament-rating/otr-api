using Database;
using Database.Entities;
using Database.Interceptors;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DataWorkerService.AutomationChecks;
using DataWorkerService.AutomationChecks.Games;
using DataWorkerService.AutomationChecks.Matches;
using DataWorkerService.AutomationChecks.Scores;
using DataWorkerService.AutomationChecks.Tournaments;
using DataWorkerService.BackgroundServices;
using DataWorkerService.Configurations;
using DataWorkerService.Processors;
using DataWorkerService.Processors.Games;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Processors.Resolvers.Implementations;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Processors.Scores;
using DataWorkerService.Processors.Tournaments;
using DataWorkerService.Services.Implementations;
using DataWorkerService.Services.Interfaces;
using DataWorkerService.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Extensions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

#region Configuration Bindings

builder.Services
    .AddOptions<PlayerFetchConfiguration>()
    .Bind(builder.Configuration.GetSection(PlayerFetchConfiguration.Position));
builder.Services
    .AddOptions<TournamentProcessingConfiguration>()
    .Bind(builder.Configuration.GetSection(TournamentProcessingConfiguration.Position));

#endregion

#region Logger Configuration

builder.Services.AddSerilog(configuration =>
{
    var connString = builder
        .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
            ConnectionStringsConfiguration.Position
        )
        .DefaultConnection;

#if DEBUG
    configuration.MinimumLevel.Debug();
#else
    configuration.MinimumLevel.Information();
#endif

    configuration
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override(
            "Microsoft.EntityFrameworkCore.Database.Command",
            LogEventLevel.Warning
        )
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.File(
            path: Path.Join("logs", "dataworker_log.log"),
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information
        )
        .WriteTo.PostgreSQL(
            connString,
            tableName: "Logs",
            needAutoCreateTable: true,
            restrictedToMinimumLevel: LogEventLevel.Warning
        );
});

builder.Services.AddLogging();

#endregion

#region Database Context

builder.Services.AddDbContext<OtrContext>(o =>
{
    o.UseNpgsql(
        builder
            .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
                ConnectionStringsConfiguration.Position
            )
            .DefaultConnection
    )
    .AddInterceptors(new AuditingInterceptor())
    .UseSnakeCaseNamingConvention();
});

// Redis lock factory (distributed resource access control)
var useRedLock = builder.Configuration.Get<OsuConfiguration>()?.EnableDistributedLocking ?? true;

if (useRedLock)
{
    var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>
    {
        new(ConnectionMultiplexer.Connect(builder.Configuration
            .BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).RedisConnection))
    });
    builder.Services.AddSingleton(redLockFactory);
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

#endregion

#region osu! API

builder.Services.AddOsuApiClient(new OsuClientOptions
{
    Configuration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position),
    UseScopedServices = false
});

#endregion

#region Repositories

builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();
builder.Services.AddScoped<IGamesRepository, GamesRepository>();
builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();

#endregion

#region Services

builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IOsuApiDataParserService, OsuApiDataParserService>();

#endregion

#region Automation Checks

builder.Services.AddSingleton<IAutomationCheck<GameScore>, ScoreMinimumCheck>();
builder.Services.AddSingleton<IAutomationCheck<GameScore>, ScoreModCheck>();
builder.Services.AddSingleton<IAutomationCheck<GameScore>, ScoreRulesetCheck>();

builder.Services.AddSingleton<IAutomationCheck<Game>, GameBeatmapUsageCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameEndTimeCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameModCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameRulesetCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameScoreCountCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameScoringTypeCheck>();
builder.Services.AddSingleton<IAutomationCheck<Game>, GameTeamTypeCheck>();

builder.Services.AddSingleton<IAutomationCheck<Match>, MatchEndTimeCheck>();
builder.Services.AddSingleton<IAutomationCheck<Match>, MatchGameCountCheck>();
builder.Services.AddSingleton<IAutomationCheck<Match>, MatchHeadToHeadCheck>();
builder.Services.AddSingleton<IAutomationCheck<Match>, MatchNamePrefixCheck>();
builder.Services.AddSingleton<IAutomationCheck<Match>, MatchTeamsIntegrityCheck>();

builder.Services.AddSingleton<IAutomationCheck<Tournament>, TournamentMatchCountCheck>();

#endregion

#region Processors

builder.Services.AddScoped<IScoreProcessorResolver, ScoreProcessorResolver>();
builder.Services.AddScoped<IProcessor<GameScore>, ScoreAutomationChecksProcessor>();
builder.Services.AddScoped<IProcessor<GameScore>, ScoreVerificationProcessor>();

builder.Services.AddScoped<IGameProcessorResolver, GameProcessorResolver>();
builder.Services.AddScoped<IProcessor<Game>, GameAutomationChecksProcessor>();
builder.Services.AddScoped<IProcessor<Game>, GameStatsProcessor>();
builder.Services.AddScoped<IProcessor<Game>, GameVerificationProcessor>();

builder.Services.AddScoped<IMatchProcessorResolver, MatchProcessorResolver>();
builder.Services.AddScoped<IProcessor<Match>, MatchDataProcessor>();
builder.Services.AddScoped<IProcessor<Match>, MatchAutomationChecksProcessor>();
builder.Services.AddScoped<IProcessor<Match>, MatchStatsProcessor>();
builder.Services.AddScoped<IProcessor<Match>, MatchVerificationProcessor>();

builder.Services.AddScoped<ITournamentProcessorResolver, TournamentProcessorResolver>();
builder.Services.AddScoped<IProcessor<Tournament>, TournamentDataProcessor>();
builder.Services.AddScoped<IProcessor<Tournament>, TournamentAutomationChecksProcessor>();
builder.Services.AddScoped<IProcessor<Tournament>, TournamentStatsProcessor>();
builder.Services.AddScoped<IProcessor<Tournament>, TournamentVerificationProcessor>();

#endregion

# region Background Services

builder.Services.AddHostedService<PlayerOsuUpdateService>();
builder.Services.AddHostedService<PlayerOsuTrackUpdateService>();
builder.Services.AddHostedService<TournamentProcessorService>();

# endregion

IHost host = builder.Build();
host.Run();
