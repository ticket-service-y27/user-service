using UserService.Application;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructurePersistence(builder.Configuration)
    .AddMigrations()
    .AddInfrastructureSecurity(builder.Configuration);

WebApplication app = builder.Build();

await app.Services.RunMigrations();
app.Run();