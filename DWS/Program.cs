using AutoMapper;
using Common.Configurations;
using Common.Constants;
using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;
using DWS.Configurations;
using DWS.Consumers;
using DWS.Services;
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

    // Register automation check classes
    builder.Services.AddScoped<ScoreAutomationChecks>();
    builder.Services.AddScoped<GameAutomationChecks>();
    builder.Services.AddScoped<MatchAutomationChecks>();
    builder.Services.AddScoped<TournamentAutomationChecks>();

    // Register automation check service
    builder.Services.AddScoped<ITournamentAutomationCheckService, TournamentAutomationCheckService>();

    // Register stats services
    builder.Services.AddScoped<IGameStatsService, GameStatsService>();
    builder.Services.AddScoped<IMatchStatsService, MatchStatsService>();
    builder.Services.AddScoped<ITournamentStatsService, TournamentStatsService>();

    // Configure MassTransit
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();
        x.AddConsumer<MatchFetchConsumer>();
        x.AddConsumer<PlayerFetchConsumer>();
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

    Log.Information("DWS (DataWorkerService) starting up...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DWS (DataWorkerService) terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
