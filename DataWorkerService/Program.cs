using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DataWorkerService.Configurations;
using DataWorkerService.Extensions;
using DataWorkerService.Services.Implementations;
using DataWorkerService.Services.Interfaces;
using DataWorkerService.Workers;
using Microsoft.EntityFrameworkCore;
using OsuApiClient;
using OsuApiClient.Configurations.Implementations;
using OsuApiClient.Extensions;
using Serilog;
using Serilog.Events;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

#region Configuration Bindings

OsuConfiguration osuConfig =
    builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);

builder.Services.AddOptions<OsuConfiguration>().Bind(builder.Configuration.GetSection(OsuConfiguration.Position));
builder.Services.AddOptions<PlayerDataWorkerConfiguration>().Bind(builder.Configuration.GetSection(PlayerDataWorkerConfiguration.Position));

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
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}"
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

#endregion

#region osu! API

builder.Services.AddOsuApiClient(new OsuClientOptions
{
    Configuration = new OsuClientConfiguration
    {
        ClientId = osuConfig.ClientId,
        ClientSecret = osuConfig.ClientSecret
    },
    UseScopedServices = false
});

#endregion

#region Repositories

builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();

#endregion

#region Services

builder.Services.AddScoped<IPlayersService, PlayersService>();

#endregion

builder.Services.AddHostedService<OsuPlayerDataWorker>();
builder.Services.AddHostedService<OsuTrackPlayerDataWorker>();

IHost host = builder.Build();
host.Run();
