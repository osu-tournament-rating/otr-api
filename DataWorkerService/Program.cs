using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DataWorkerService;
using DataWorkerService.Configurations;
using DataWorkerService.Extensions.Utilities;
using Microsoft.EntityFrameworkCore;
using OsuApiClient.Configurations.Implementations;
using OsuApiClient.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

OsuConfiguration osuConfig =
    builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);

builder.Services.AddOptions<OsuConfiguration>().Bind(builder.Configuration.GetSection(OsuConfiguration.Position));

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

builder.Services.AddOsuApiClient(new OsuClientConfiguration
{
    ClientId = osuConfig.ClientId,
    ClientSecret = osuConfig.ClientSecret
});

builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
