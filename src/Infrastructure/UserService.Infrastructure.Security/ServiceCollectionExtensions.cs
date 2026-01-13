using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions.Security;
using UserService.Infrastructure.Security.JwtToken;
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

        services.Configure<JwtAuthenticationOptions>(
            configuration.GetSection("Infrastructure:Security:JwtOptions"));
        services.AddSingleton<IJwtTokenCreator, JwtTokenCreator>();

        return services;
    }
}