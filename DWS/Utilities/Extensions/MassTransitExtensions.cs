using MassTransit;

namespace DWS.Utilities.Extensions;

public static class MassTransitExtensions
{
    /// <summary>
    /// Configures a receive endpoint for rate-limited API consumers.
    /// Ensures sequential message processing to respect external API rate limits.
    /// </summary>
    /// <param name="endpointConfigurator">The endpoint configurator</param>
    /// <param name="concurrentMessageLimit">Number of concurrent consumers</param>
    private static void ConfigureRateLimitedEndpoint(this IReceiveEndpointConfigurator endpointConfigurator, int concurrentMessageLimit = 1)
    {
        // Enable priority queue with max priority level of 10 if this is a RabbitMQ endpoint
        // This allows high priority messages to be processed first
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMqEndpoint)
        {
            rabbitMqEndpoint.EnablePriority(10);
        }

        // Process only one message at a time to ensure we respect the API rate limit
        // This also ensures strict priority ordering
        endpointConfigurator.PrefetchCount = 1;

        // Limit concurrent message processing
        // This controls how many consumer instances process messages at a time
        endpointConfigurator.ConcurrentMessageLimit = concurrentMessageLimit;
    }

    /// <summary>
    /// Configures a receive endpoint for osu! API consumers with appropriate rate limiting.
    /// </summary>
    /// <param name="cfg">The RabbitMQ bus factory configurator</param>
    /// <param name="context">The bus registration context</param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="concurrentMessageLimit">Number of concurrent consumers</param>
    public static void ReceiveOsuApiEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName,
        int concurrentMessageLimit = 1) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureRateLimitedEndpoint(concurrentMessageLimit);
            e.ConfigureConsumer<TConsumer>(context);
        });
    }

    /// <summary>
    /// Configures a receive endpoint for osu!track API consumers with appropriate rate limiting.
    /// </summary>
    /// <param name="cfg">The RabbitMQ bus factory configurator</param>
    /// <param name="context">The bus registration context</param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="concurrentMessageLimit">Number of concurrent consumers</param>
    public static void ReceiveOsuTrackApiEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName,
        int concurrentMessageLimit = 1) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureRateLimitedEndpoint(concurrentMessageLimit);
            e.ConfigureConsumer<TConsumer>(context);
        });
    }

    /// <summary>
    /// Configures a receive endpoint for automation check consumers with priority queue support.
    /// Uses sequential processing to prevent concurrent Entity Framework modifications.
    /// </summary>
    /// <param name="cfg">The RabbitMQ bus factory configurator</param>
    /// <param name="context">The bus registration context</param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="concurrentMessageLimit">Number of concurrent consumers</param>
    public static void ReceiveAutomationCheckEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName,
        int concurrentMessageLimit = 1) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            // Enable priority queue with max priority level of 10
            e.EnablePriority(10);

            // Configure concurrent message processing based on configuration
            // Lower values prevent Entity Framework conflicts but reduce throughput
            // Higher values increase throughput but may cause conflicts when modifying shared entities
            e.PrefetchCount = concurrentMessageLimit;
            e.ConcurrentMessageLimit = concurrentMessageLimit;
            e.ConfigureConsumer<TConsumer>(context);
        });
    }

    /// <summary>
    /// Configures a receive endpoint for stats processing consumers with priority queue support.
    /// Uses sequential processing to prevent concurrent Entity Framework modifications.
    /// </summary>
    /// <param name="cfg">The RabbitMQ bus factory configurator</param>
    /// <param name="context">The bus registration context</param>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="concurrentMessageLimit">Number of concurrent consumers</param>
    public static void ReceiveStatsEndpoint<TConsumer>(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        string queueName,
        int concurrentMessageLimit = 1) where TConsumer : class, IConsumer
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            // Enable priority queue with max priority level of 10
            e.EnablePriority(10);

            // Configure concurrent message processing based on configuration
            // Higher values increase throughput but may cause conflicts when modifying shared entities
            // and risk external API rate limit violations
            e.PrefetchCount = concurrentMessageLimit;
            e.ConcurrentMessageLimit = concurrentMessageLimit;
            e.ConfigureConsumer<TConsumer>(context);
        });
    }
}
