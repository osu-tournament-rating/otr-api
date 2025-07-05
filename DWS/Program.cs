using Common.Configurations;
using Common.Constants;
using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
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

    // Configure RabbitMQ
    builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.Position));

    // Register repositories
    builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();
    builder.Services.AddScoped<IBeatmapsetsRepository, BeatmapsetsRepository>();
    builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
    builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
    builder.Services.AddScoped<IGamesRepository, GamesRepository>();
    builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();

    // Register services
    builder.Services.AddScoped<IBeatmapsetFetchService, BeatmapsetFetchService>();
    builder.Services.AddScoped<IMatchFetchService, MatchFetchService>();

    // Configure MassTransit
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();
        x.AddConsumer<MatchFetchConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            RabbitMqConfiguration rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

            cfg.Host(rabbitMqConfig.Host, "/", h =>
            {
                h.Username(rabbitMqConfig.Username);
                h.Password(rabbitMqConfig.Password);
            });

            // Configure endpoint for BeatmapFetchConsumer with rate limiting
            // This ensures only one message is processed at a time to respect osu! API rate limits
            cfg.ReceiveOsuApiEndpoint<BeatmapFetchConsumer>(context, QueueConstants.Osu.Beatmaps);

            // Configure endpoint for MatchFetchConsumer with rate limiting
            cfg.ReceiveOsuApiEndpoint<MatchFetchConsumer>(context, QueueConstants.Osu.Matches);

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
