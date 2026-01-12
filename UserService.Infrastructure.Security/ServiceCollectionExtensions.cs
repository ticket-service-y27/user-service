using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions.Security;
using UserService.Infrastructure.Security.Options;
using UserService.Infrastructure.Security.Password;

namespace UserService.Infrastructure.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PasswordHasherOptions>(
            configuration.GetSection("Infrastructure:Security:PasswordHasherOptions"));

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}