using Itmo.Dev.Platform.Events;
using UserService.Application;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Security;
using UserService.Presentation.Grpc;
using UserService.Presentation.Grpc.Services;
using UserService.Presentation.Kafka;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructurePersistence(builder.Configuration)
    .AddMigrations()
    .AddInfrastructureSecurity(builder.Configuration)
    .AddPresentationGrpc()
    .AddPresentationKafka(builder.Configuration);

builder.Services.AddPlatformEvents(events => events
    .AddPresentationKafkaEventHandlers());

WebApplication app = builder.Build();
await app.Services.RunMigrations();
app.MapGrpcService<UserServiceGrpc>();

app.Run();