using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Users.Kafka.Contracts;
using UserService.Application.Contracts.Users.Events;

namespace UserService.Presentation.Kafka.ProducerHandlers.Users;

public class UserUnblockedHandler : IEventHandler<UserUnblockedEvent>
{
    private readonly IKafkaMessageProducer<UserEventKey, UserEventValue> _producer;

    public UserUnblockedHandler(IKafkaMessageProducer<UserEventKey, UserEventValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(UserUnblockedEvent evt, CancellationToken cancellationToken)
    {
        var key = new UserEventKey { UserId = evt.UserId };

        var value = new UserEventValue
        {
            UserUnblocked = new UserEventValue.Types.UserUnblocked
            {
                UserId = evt.UserId,
                UnblockedAt = evt.UnblockedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<UserEventKey, UserEventValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}