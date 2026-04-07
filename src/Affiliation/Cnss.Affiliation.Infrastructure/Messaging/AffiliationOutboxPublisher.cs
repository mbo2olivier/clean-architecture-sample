using Cnss.Shared.Infrastructure.Messaging;

namespace Cnss.Affiliation.Infrastructure.Messaging;

public sealed class AffiliationOutboxPublisher : RabbitMqOutboxPublisher
{
    public AffiliationOutboxPublisher(Cnss.Shared.Infrastructure.Configuration.RabbitMqOptions options)
        : base(options)
    {
    }
}
