using Microsoft.Extensions.DependencyInjection;

namespace UserService.Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection services)
    {
        return services;
    }
}