using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Users.Kafka.Contracts;
using UserService.Application.Contracts.Users.Events;

namespace UserService.Presentation.Kafka.ProducerHandlers.Users;

public class UserBlockedHandler : IEventHandler<UserBlockedEvent>
{
    private readonly IKafkaMessageProducer<UserEventKey, UserEventValue> _producer;

    public UserBlockedHandler(IKafkaMessageProducer<UserEventKey, UserEventValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(UserBlockedEvent evt, CancellationToken cancellationToken)
    {
        var key = new UserEventKey { UserId = evt.UserId };

        var value = new UserEventValue
        {
            UserBlocked = new UserEventValue.Types.UserBlocked
            {
                UserId = evt.UserId,
                BlockedAt = evt.BlockedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<UserEventKey, UserEventValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}