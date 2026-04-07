using System.Text;
using Cnss.Affiliation.Infrastructure.Configuration;
using RabbitMQ.Client;

namespace Cnss.Affiliation.Infrastructure.Messaging;

public sealed class AffiliationOutboxPublisher
{
    private readonly RabbitMqOptions _options;

    public AffiliationOutboxPublisher(RabbitMqOptions options)
    {
        _options = options;
    }

    public async Task PublishAsync(string routingKey, string payload, string eventType, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            Persistent = true,
            Type = eventType,
            ContentType = "application/json"
        };

        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
