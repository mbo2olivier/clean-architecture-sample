namespace Cnss.Shared.Infrastructure.Configuration;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = "cnss";

    public string Password { get; set; } = "cnss";

    public string ExchangeName { get; set; } = "cnss.domain.events";

    public int BatchSize { get; set; } = 20;

    public int PollingIntervalInSeconds { get; set; } = 5;

    public int LockDurationInSeconds { get; set; } = 30;
}
