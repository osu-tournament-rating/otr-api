namespace Common.Configurations;

public class RabbitMqConfiguration
{
    public const string Position = "RabbitMq";

    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
