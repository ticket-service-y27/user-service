using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application;
using UserService.Application.Contracts;
using UserService.Application.Models.Users;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Options;
using UserService.Infrastructure.Security;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

IServiceCollection services = new ServiceCollection();

services
    .Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"))
    .AddApplicationServices()
    .AddInfrastructurePersistence(configuration)
    .AddMigrations()
    .AddInfrastructureSecurity(configuration);

ServiceProvider provider = services.BuildServiceProvider();

await provider.RunMigrations();

CancellationToken ct = new CancellationTokenSource().Token;

IUserService userService = provider.GetRequiredService<IUserService>();

try
{
    long id = await userService.CreateUserAsync("test", "email", "pass", ct);
    Console.WriteLine($"USER CREATED, id={id}");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

try
{
    await userService.AssignUserRoleAsync(55, UserRole.Organizer, ct);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

try
{
    string token = await userService.LogInByNicknameAsync("admin", "admin", ct);
    Console.WriteLine($"USER LOG IN : token = {token}");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

try
{
    await userService.BlockUserByIdAsync(30, ct);
    Console.WriteLine("USER BLOCKED");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}