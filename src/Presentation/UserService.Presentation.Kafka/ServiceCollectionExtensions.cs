using Itmo.Dev.Platform.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Kafka.Contracts;

namespace UserService.Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        const string producerKey = "Presentation:Kafka:Producers";

        services.AddPlatformKafka(kafka => kafka
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddProducer(producer => producer
                .WithKey<UserEventKey>()
                .WithValue<UserEventValue>()
                .WithConfiguration(configuration.GetSection($"{producerKey}:UserCreation"))
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