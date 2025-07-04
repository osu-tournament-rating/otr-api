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
        .Filter.ByExcluding(e => e.MessageTemplate.Text.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
        .WriteTo.Console());

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
    builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();

    // Register services
    builder.Services.AddScoped<IBeatmapsetFetchService, BeatmapsetFetchService>();

    // Configure MassTransit
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();

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
