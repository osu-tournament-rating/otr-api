using Database;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using DataWorkerService;
using DataWorkerService.Configurations;
using DataWorkerService.Extensions.Utilities;
using Microsoft.EntityFrameworkCore;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

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

builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
