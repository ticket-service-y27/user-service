using UserService.Application;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Security;
using UserService.Presentation.Grpc;
using UserService.Presentation.Grpc.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructurePersistence(builder.Configuration)
    .AddMigrations()
    .AddInfrastructureSecurity(builder.Configuration)
    .AddPresentationGrpc();

WebApplication app = builder.Build();
await app.Services.RunMigrations();
app.MapGrpcService<UserServiceGrpc>();

app.Run();