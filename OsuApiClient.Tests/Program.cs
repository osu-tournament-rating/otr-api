using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OsuApiClient;
using OsuApiClient.Configurations.Implementations;
using OsuApiClient.Extensions;
using OsuApiClient.Tests;
using Serilog;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddJsonFile("appsettings.json"));

builder.UseSerilog((_, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.ConfigureServices((ctx, services) =>
{
    var clientConfiguration = new OsuClientConfiguration();
    ctx.Configuration.GetSection("OsuClient").Bind(clientConfiguration);

    services.AddOsuApiClient(new OsuClientOptions
    {
        Configuration = clientConfiguration,
        UseScopedServices = true
    }
    );

    services.AddHostedService<ClientTestService>();
});

builder.Build().Run();
