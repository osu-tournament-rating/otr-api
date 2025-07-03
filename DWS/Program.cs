using Database;
using DWS.Consumers;
using DWS.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OsuApiClient.Configurations.Implementations;
using OsuApiClient.Extensions;
using Serilog;

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
        .WriteTo.Console());

    // Configure Entity Framework
    builder.Services.AddDbContext<OtrContext>((services, sqlOptions) =>
    {
        sqlOptions
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .LogTo(Log.Logger.Information, LogLevel.Information)
            .UseSnakeCaseNamingConvention();
    });

    // Configure osu! API client
    string osuClientId = builder.Configuration["Osu:ClientId"] ?? throw new InvalidOperationException("Osu:ClientId is not configured");
    string osuClientSecret = builder.Configuration["Osu:ClientSecret"] ?? throw new InvalidOperationException("Osu:ClientSecret is not configured");

    builder.Services.AddOsuApiClient(new OsuClientConfiguration
    {
        ClientId = long.Parse(osuClientId),
        ClientSecret = osuClientSecret,
        RedirectUrl = "http://localhost:5075/api/v1/auth/callback" // Not used by DWS but required
    });

    // Register services
    builder.Services.AddScoped<IBeatmapFetchService, BeatmapFetchService>();

    // Configure MassTransit
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<BeatmapFetchConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            IConfiguration configuration = context.GetRequiredService<IConfiguration>();
            string rabbitMqHost = configuration["RabbitMq:Host"] ?? "localhost";
            string rabbitMqUsername = configuration["RabbitMq:Username"] ?? "admin";
            string rabbitMqPassword = configuration["RabbitMq:Password"] ?? "admin";

            cfg.Host(rabbitMqHost, "/", h =>
            {
                h.Username(rabbitMqUsername);
                h.Password(rabbitMqPassword);
            });

            cfg.ConfigureEndpoints(context);

            // Configure retry policy
            cfg.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ));

            // Configure error handling
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
