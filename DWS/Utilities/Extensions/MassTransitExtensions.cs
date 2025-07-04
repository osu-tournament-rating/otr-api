using MassTransit;

namespace DWS.Utilities.Extensions;

public static class MassTransitExtensions
{
    /// <summary>
    /// Configures a receive endpoint for rate-limited API consumers.
    /// Ensures sequential message processing to respect external API rate limits.
    /// </summary>
    private static void ConfigureRateLimitedEndpoint(this IReceiveEndpointConfigurator endpointConfigurator)
    {
        // Process only one message at a time to ensure we respect the API rate limit
        endpointConfigurator.PrefetchCount = 1;

        // Limit concurrent message processing to 1
        // This ensures that only one consumer instance processes messages at a time
        endpointConfigurator.ConcurrentMessageLimit = 1;
    }

    /// <summary>
    /// Configures a receive endpoint for osu! API consumers with appropriate rate limiting.
    /// </summary>
    public static void ReceiveOsuApiEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureRateLimitedEndpoint();
            e.ConfigureConsumer<TConsumer>(context);
        });
    }

    /// <summary>
    /// Configures a receive endpoint for osu!track API consumers with appropriate rate limiting.
    /// </summary>
    public static void ReceiveOsuTrackApiEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureRateLimitedEndpoint();
            e.ConfigureConsumer<TConsumer>(context);
        });
    }
}
