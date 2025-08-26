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
using StackExchange.Redis;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

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

    builder.Services.AddDbContext<OtrContext>((_, sqlOptions) =>
    {
        sqlOptions
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .LogTo(Log.Logger.Information, LogLevel.Information)
            .UseSnakeCaseNamingConvention();
    });

    builder.Services.AddOsuApiClient(new OsuClientOptions
    {
        Configuration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position)
    });

    builder.Services.AddAutoMapper(typeof(DwsMapperProfile));

    builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.Position));

    builder.Services.AddOptionsWithValidateOnStart<PlayerUpdateServiceConfiguration>()
        .Bind(builder.Configuration.GetSection(PlayerUpdateServiceConfiguration.Position))
        .ValidateDataAnnotations();

    builder.Services.AddOptionsWithValidateOnStart<PlayerOsuTrackUpdateServiceConfiguration>()
        .Bind(builder.Configuration.GetSection(PlayerOsuTrackUpdateServiceConfiguration.Position))
        .ValidateDataAnnotations();

    builder.Services.AddOptionsWithValidateOnStart<MessageDeduplicationConfiguration>()
        .Bind(builder.Configuration.GetSection(MessageDeduplicationConfiguration.Position))
        .ValidateDataAnnotations();

    builder.Services.AddOptionsWithValidateOnStart<ConnectionStringsConfiguration>()
        .Bind(builder.Configuration.GetSection(ConnectionStringsConfiguration.Position))
        .ValidateDataAnnotations();

    builder.Services.AddOptionsWithValidateOnStart<ConsumerConcurrencyConfiguration>()
        .Bind(builder.Configuration.GetSection(ConsumerConcurrencyConfiguration.Position))
        .ValidateDataAnnotations();

    builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
    {
        ConnectionStringsConfiguration connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStringsConfiguration>>().Value;
        ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Connecting to Redis at: {RedisConnection}", connectionStrings.RedisConnection);

        var configurationOptions = ConfigurationOptions.Parse(connectionStrings.RedisConnection);
        configurationOptions.AbortOnConnectFail = false;
        configurationOptions.ConnectRetry = 3;
        configurationOptions.ConnectTimeout = 5000;
        configurationOptions.SyncTimeout = 5000;

        var connection = ConnectionMultiplexer.Connect(configurationOptions);

        connection.ConnectionFailed += (_, args) =>
        {
            logger.LogError("Redis connection failed: {EndPoint} - {FailureType}", args.EndPoint, args.FailureType);
        };

        connection.ConnectionRestored += (_, args) =>
        {
            logger.LogInformation("Redis connection restored: {EndPoint}", args.EndPoint);
        };

        return connection;
    });

    builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();
    builder.Services.AddScoped<IBeatmapsetsRepository, BeatmapsetsRepository>();
    builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
    builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
    builder.Services.AddScoped<IGamesRepository, GamesRepository>();
    builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();
    builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();

    builder.Services.AddSingleton<IMessageDeduplicationService, MessageDeduplicationService>();
    builder.Services.AddScoped<IBeatmapsetFetchService, BeatmapsetFetchService>();
    builder.Services.AddScoped<IMatchFetchService, MatchFetchService>();
    builder.Services.AddScoped<IPlayerFetchService, PlayerFetchService>();
    builder.Services.AddScoped<IPlayerOsuTrackFetchService, PlayerOsuTrackFetchService>();
    builder.Services.AddScoped<ITournamentDataCompletionService, TournamentDataCompletionService>();

    builder.Services.AddScoped<ScoreAutomationChecks>();
    builder.Services.AddScoped<GameAutomationChecks>();
    builder.Services.AddScoped<MatchAutomationChecks>();
    builder.Services.AddScoped<TournamentAutomationChecks>();

    builder.Services.AddScoped<ITournamentAutomationCheckService, TournamentAutomationCheckService>();
    builder.Services.AddScoped<ITournamentStatsService, TournamentStatsService>();

    builder.Services.AddScoped<IStatsCalculator, TournamentStatsCalculator>();

    builder.Services.AddHostedService<PlayerUpdateBackgroundService>();
    builder.Services.AddHostedService<PlayerOsuTrackUpdateBackgroundService>();

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();
        x.AddConsumer<MatchFetchConsumer>();
        x.AddConsumer<PlayerFetchConsumer>();
        x.AddConsumer<PlayerOsuTrackFetchConsumer>();
        x.AddConsumer<TournamentAutomationCheckConsumer>();
        x.AddConsumer<TournamentStatsConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            RabbitMqConfiguration rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            ConsumerConcurrencyConfiguration concurrencyConfig = context.GetRequiredService<IOptions<ConsumerConcurrencyConfiguration>>().Value;

            cfg.Host(rabbitMqConfig.Host, "/", h =>
            {
                h.Username(rabbitMqConfig.Username);
                h.Password(rabbitMqConfig.Password);
            });

            cfg.ReceiveOsuApiEndpoint<BeatmapFetchConsumer>(context, QueueConstants.Osu.Beatmaps, concurrencyConfig.BeatmapFetchConsumers);
            cfg.ReceiveOsuApiEndpoint<MatchFetchConsumer>(context, QueueConstants.Osu.Matches, concurrencyConfig.MatchFetchConsumers);
            cfg.ReceiveOsuApiEndpoint<PlayerFetchConsumer>(context, QueueConstants.Osu.Players, concurrencyConfig.PlayerFetchConsumers);
            cfg.ReceiveOsuTrackApiEndpoint<PlayerOsuTrackFetchConsumer>(context, QueueConstants.OsuTrack.Players, concurrencyConfig.PlayerOsuTrackFetchConsumers);

            cfg.ReceiveAutomationCheckEndpoint<TournamentAutomationCheckConsumer>(context, QueueConstants.AutomatedChecks.Tournaments, concurrencyConfig.TournamentAutomationCheckConsumers);

            cfg.ReceiveStatsEndpoint<TournamentStatsConsumer>(context, QueueConstants.Stats.Tournaments, concurrencyConfig.TournamentStatsConsumers);

            cfg.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ));
        });
    });

    IHost host = builder.Build();

    Log.Information("DWS starting up...");

    // Clear all message deduplication keys
    var deduplicationService = host.Services.GetRequiredService<IMessageDeduplicationService>();
    await deduplicationService.ClearAsync();

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
