using Microsoft.Extensions.DependencyInjection;

namespace UserService.Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection services)
    {
        services.AddGrpc(grpc => grpc.Interceptors.Add<GrpcServerInterceptor>());
        return services;
    }
}