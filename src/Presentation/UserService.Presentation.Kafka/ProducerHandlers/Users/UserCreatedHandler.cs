using Google.Protobuf.WellKnownTypes;
using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Users.Kafka.Contracts;
using UserService.Application.Contracts.Users.Events;

namespace UserService.Presentation.Kafka.ProducerHandlers.Users;

public class UserCreatedHandler : IEventHandler<UserCreatedEvent>
{
    private readonly IKafkaMessageProducer<UserCreationKey, UserCreationValue> _producer;

    public UserCreatedHandler(IKafkaMessageProducer<UserCreationKey, UserCreationValue> producer)
    {
        _producer = producer;
    }

    public async ValueTask HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        var key = new UserCreationKey { UserId = evt.UserId };

        var value = new UserCreationValue
        {
            UserCreated = new UserCreationValue.Types.UserCreated
            {
                UserId = evt.UserId,
                CreatedAt = evt.CreatedAt.ToTimestamp(),
            },
        };

        var message = new KafkaProducerMessage<UserCreationKey, UserCreationValue>(key, value);
        await _producer.ProduceAsync(message, cancellationToken);
    }
}