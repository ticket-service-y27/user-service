using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Application.Abstractions.LoyaltySystem.Managers;
using UserService.Infrastructure.Loyalty.LoyaltySystem;
using UserService.Infrastructure.Loyalty.LoyaltySystem.Managers;
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
        services.AddScoped<ILoyaltyPeriodManager, LoyaltyPeriodManager>();

        services.Configure<LoyaltyBackgroundServiceOptions>(
            configuration.GetSection("Infrastructure:LoyaltySystem:BackgroundService:UpdatePeriod"));
        services.AddHostedService<LoyaltyPeriodBackgroundService>();

        return services;
    }
}