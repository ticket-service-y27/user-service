using Itmo.Dev.Platform.Kafka.Consumer;
using Loyalty.Kafka.Contracts;
using UserService.Application.Contracts;

namespace UserService.Presentation.Kafka.ConsumerHandlers;

public class PaymentSumConsumerHandler : IKafkaConsumerHandler<PaymentsSumKey, PaymentsSumValue>
{
    private readonly IUserService _userService;

    public PaymentSumConsumerHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<PaymentsSumKey, PaymentsSumValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<PaymentsSumKey, PaymentsSumValue> message in messages)
        {
            await _userService.RecalculateUserLoyaltyAsync(
                userId: message.Value.UserId,
                totalSpent: message.Value.PaymentsSum,
                calculatedAt: message.Value.PublishedAt.ToDateTimeOffset(),
                cancellationToken);
        }
    }
}