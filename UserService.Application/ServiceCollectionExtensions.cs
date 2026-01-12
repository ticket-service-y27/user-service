using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Contracts;
using UserService.Application.Services;

namespace UserService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserApplicationService>();
        return services;
    }
}