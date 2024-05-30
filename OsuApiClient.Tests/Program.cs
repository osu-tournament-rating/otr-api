using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OsuApiClient;
using OsuApiClient.Configurations.Implementations;
using OsuApiClient.Extensions;
using OsuApiClient.Tests;
using OsuApiClient.Tests.Tests;
using Serilog;

IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder.UseSerilog((_, configuration) =>
{
    Console.OutputEncoding = Encoding.UTF8;
    configuration
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.ConfigureServices((ctx, services) =>
{
    // Configure client
    var clientConfiguration = new OsuClientConfiguration();
    ctx.Configuration.GetSection("OsuClient").Bind(clientConfiguration);

    services.AddOsuApiClient(new OsuClientOptions
    {
        Configuration = clientConfiguration,
        UseScopedServices = true
    });

    // Tests
    services.AddSingleton<IOsuClientTest, ClientAuthorizationTest>();
    services.AddSingleton<IOsuClientTest, RefreshTokenAuthorizationTest>();
    services.AddSingleton<IOsuClientTest, CodeAuthorizationTest>();
    services.AddSingleton<IOsuClientTest, GetUserTest>();

    services.AddHostedService<ClientTestService>();
});

builder.Build().Run();
