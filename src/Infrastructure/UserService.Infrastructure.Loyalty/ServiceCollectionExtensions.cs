using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Infrastructure.Loyalty.LoyaltySystem;
using UserService.Infrastructure.Loyalty.LoyaltySystem.LoyaltySystemManagers;
using UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

namespace UserService.Infrastructure.Loyalty;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLoyaltySystem(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LoyaltyLevelCalculatorOptions>(
            configuration.GetSection("Infrastructure:LoyaltySystem"));

        services.AddSingleton<ILoyaltyLevelCalculator, LoyaltyLevelCalculator>();
        services.AddScoped<IUserLoyaltyManager, UserLoyaltyManager>();

        return services;
    }
}