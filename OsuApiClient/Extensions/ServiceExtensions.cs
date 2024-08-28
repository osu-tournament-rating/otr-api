using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OsuApiClient.Configurations.Interfaces;
using OsuApiClient.Net.Requests.RequestHandler;

namespace OsuApiClient.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds the OsuApiClient and related environment to the <see cref="IServiceCollection"/>
    /// </summary>
    public static IServiceCollection AddOsuApiClient(
        this IServiceCollection services,
        IOsuClientConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddOsuApiClient(configuration, true);
    }

    /// <summary>
    /// Adds the OsuApiClient and related environment to the <see cref="IServiceCollection"/>
    /// </summary>
    public static IServiceCollection AddOsuApiClient(
        this IServiceCollection services,
        OsuClientOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.Configuration is null)
        {
            throw new InvalidOperationException($"The client configuration cannot be null");
        }

        return services.AddOsuApiClient(options.Value.Configuration, options.Value.UseScopedServices);
    }

    /// <summary>
    /// Adds the OsuApiClient and related environment to the <see cref="IServiceCollection"/>
    /// </summary>
    private static IServiceCollection AddOsuApiClient(
        this IServiceCollection services,
        IOsuClientConfiguration configuration,
        bool scoped
    )
    {
        services.AddSingleton(configuration);

        services.TryAddSingleton<IRequestHandler, DefaultRequestHandler>();

        if (scoped)
        {
            services.TryAddScoped<IOsuClient, OsuClient>();
        }
        else
        {
            services.TryAddSingleton<IOsuClient, OsuClient>();
        }

        return services;
    }
}
