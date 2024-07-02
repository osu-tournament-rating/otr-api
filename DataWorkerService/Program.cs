using Database;
using Database.Entities;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DataWorkerService.BackgroundServices;
using DataWorkerService.Configurations;
using DataWorkerService.Extensions;
using DataWorkerService.Processors;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Processors.Resolvers.Implementations;
using DataWorkerService.Processors.Resolvers.Interfaces;
using DataWorkerService.Processors.Tournaments;
using DataWorkerService.Services.Implementations;
using DataWorkerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Extensions;
using Serilog;
using Serilog.Events;

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
    );
});

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

# region Processors

builder.Services.AddScoped<ITournamentProcessorResolver, TournamentProcessorResolver>();
builder.Services.AddScoped<IProcessor<Tournament>, TournamentDataProcessor>();

builder.Services.AddScoped<IMatchProcessorResolver, MatchProcessorResolver>();
builder.Services.AddScoped<IProcessor<Match>, MatchDataProcessor>();

# endregion

# region Background Services

builder.Services.AddHostedService<PlayerOsuUpdateService>();
builder.Services.AddHostedService<PlayerOsuTrackUpdateService>();
builder.Services.AddHostedService<TournamentProcessorService>();

# endregion

IHost host = builder.Build();
host.Run();
