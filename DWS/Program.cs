using Common.Configurations;
using Common.Constants;
using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;
using DWS.Calculators;
using DWS.Configurations;
using DWS.Consumers;
using DWS.Services;
using DWS.Services.Implementations;
using DWS.Services.Interfaces;
using DWS.Utilities.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OsuApiClient;
using OsuApiClient.Extensions;
using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
        .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
        .MinimumLevel.Override("MassTransit.Messages", LogEventLevel.Warning)
        .Filter.ByExcluding(e => e.MessageTemplate.Text.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Configure Entity Framework
    builder.Services.AddDbContext<OtrContext>((_, sqlOptions) =>
    {
        sqlOptions
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .LogTo(Log.Logger.Information, LogLevel.Information)
            .UseSnakeCaseNamingConvention();
    });

    // Configure osu! API client
    builder.Services.AddOsuApiClient(new OsuClientOptions
    {
        Configuration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position)
    });

    // Configure AutoMapper
    builder.Services.AddAutoMapper(typeof(DwsMapperProfile));

    // Configure RabbitMQ
    builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.Position));

    // Configure Player Update Service
    builder.Services.AddOptionsWithValidateOnStart<PlayerUpdateServiceConfiguration>()
        .Bind(builder.Configuration.GetSection(PlayerUpdateServiceConfiguration.Position))
        .ValidateDataAnnotations();

    // Configure Player osu!track Update Service
    builder.Services.AddOptionsWithValidateOnStart<PlayerOsuTrackUpdateServiceConfiguration>()
        .Bind(builder.Configuration.GetSection(PlayerOsuTrackUpdateServiceConfiguration.Position))
        .ValidateDataAnnotations();

    // Register repositories
    builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();
    builder.Services.AddScoped<IBeatmapsetsRepository, BeatmapsetsRepository>();
    builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
    builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
    builder.Services.AddScoped<IGamesRepository, GamesRepository>();
    builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();
    builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();

    // Register services
    builder.Services.AddScoped<IBeatmapsetFetchService, BeatmapsetFetchService>();
    builder.Services.AddScoped<IMatchFetchService, MatchFetchService>();
    builder.Services.AddScoped<IPlayerFetchService, PlayerFetchService>();
    builder.Services.AddScoped<IPlayerOsuTrackFetchService, PlayerOsuTrackFetchService>();
    builder.Services.AddScoped<ITournamentDataCompletionService, TournamentDataCompletionService>();

    // Register automation check classes
    builder.Services.AddScoped<ScoreAutomationChecks>();
    builder.Services.AddScoped<GameAutomationChecks>();
    builder.Services.AddScoped<MatchAutomationChecks>();
    builder.Services.AddScoped<TournamentAutomationChecks>();

    builder.Services.AddScoped<ITournamentAutomationCheckService, TournamentAutomationCheckService>();
    builder.Services.AddScoped<ITournamentStatsService, TournamentStatsService>();

    // Register calculator for functional statistics processing
    builder.Services.AddScoped<IStatsCalculator, TournamentStatsCalculator>();

    // Register background services
    builder.Services.AddHostedService<PlayerUpdateBackgroundService>();
    builder.Services.AddHostedService<PlayerOsuTrackUpdateBackgroundService>();

    // Configure MassTransit
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();
        x.AddConsumer<MatchFetchConsumer>();
        x.AddConsumer<PlayerFetchConsumer>();
        x.AddConsumer<PlayerOsuTrackFetchConsumer>();
        x.AddConsumer<TournamentAutomationCheckConsumer>();
        x.AddConsumer<TournamentStatsConsumer>();
        x.AddConsumer<TournamentProcessedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            RabbitMqConfiguration rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

            cfg.Host(rabbitMqConfig.Host, "/", h =>
            {
                h.Username(rabbitMqConfig.Username);
                h.Password(rabbitMqConfig.Password);
            });

            // Rate-limited consumers
            cfg.ReceiveOsuApiEndpoint<BeatmapFetchConsumer>(context, QueueConstants.Osu.Beatmaps);
            cfg.ReceiveOsuApiEndpoint<MatchFetchConsumer>(context, QueueConstants.Osu.Matches);
            cfg.ReceiveOsuApiEndpoint<PlayerFetchConsumer>(context, QueueConstants.Osu.Players);
            cfg.ReceiveOsuTrackApiEndpoint<PlayerOsuTrackFetchConsumer>(context, QueueConstants.OsuTrack.Players);

            // Automation check consumer (tournament-only)
            cfg.ReceiveAutomationCheckEndpoint<TournamentAutomationCheckConsumer>(context, QueueConstants.AutomatedChecks.Tournaments);

            // Stats processing consumer (tournament-only)
            cfg.ReceiveStatsEndpoint<TournamentStatsConsumer>(context, QueueConstants.Stats.Tournaments);

            // Tournament processed notification consumer
            cfg.ReceiveStatsEndpoint<TournamentProcessedConsumer>(context, QueueConstants.Processing.TournamentsProcessed);

            // Configure retry policy
            cfg.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ));
        });
    });

    IHost host = builder.Build();

    Log.Information("DWS starting up...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DWS terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
