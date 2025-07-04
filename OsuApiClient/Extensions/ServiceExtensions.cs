using Microsoft.Extensions.DependencyInjection;
using OsuApiClient.Configurations.Interfaces;
using OsuApiClient.Net.Requests.RequestHandler;

namespace OsuApiClient.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds the OsuApiClient and related environment to the <see cref="IServiceCollection"/>
    /// </summary>
    public static void AddOsuApiClient(
        this IServiceCollection services,
        IOsuClientConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton(configuration);
        services.AddSingleton<IRequestHandler, DefaultRequestHandler>();
        services.AddScoped<IOsuClient, OsuClient>();
    }

    /// <summary>
    /// Adds the OsuApiClient and related environment to the <see cref="IServiceCollection"/>
    /// </summary>
    public static void AddOsuApiClient(
        this IServiceCollection services,
        OsuClientOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.Configuration is null)
        {
            throw new InvalidOperationException("The client configuration cannot be null");
        }

        services.AddOsuApiClient(options.Value.Configuration);
    }
}
