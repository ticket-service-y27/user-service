using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Loyalty.Kafka.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Kafka.Contracts;
using UserService.Presentation.Kafka.ConsumerHandlers;

namespace UserService.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";
        const string producerKey = "Presentation:Kafka:Producers";

        services.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(consumer => consumer
                .WithKey<PaymentsSumKey>()
                .WithValue<PaymentsSumValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:PaymentsSum"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<PaymentSumConsumerHandler>())
            .AddProducer(producer => producer
                .WithKey<UserEventKey>()
                .WithValue<UserEventValue>()
                .WithConfiguration(configuration.GetSection($"{producerKey}:UserEvents"))
                .SerializeKeyWithProto()
                .SerializeValueWithProto()));

        return services;
    }

    public static IEventsConfigurationBuilder AddPresentationKafkaEventHandlers(
        this IEventsConfigurationBuilder builder)
    {
        return builder.AddHandlersFromAssemblyContaining<IKafkaAssemblyMarker>();
    }
}