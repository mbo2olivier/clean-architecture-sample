using Cnss.Shared.Infrastructure.Messaging;

namespace Cnss.Cotisation.Infrastructure.Messaging;

public sealed class CotisationOutboxPublisher : RabbitMqOutboxPublisher
{
    public CotisationOutboxPublisher(Cnss.Shared.Infrastructure.Configuration.RabbitMqOptions options)
        : base(options)
    {
    }
}
