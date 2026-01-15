using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users.Enums;
using UserService.Infrastructure.Persistence.Options;
using UserService.Infrastructure.Persistence.Repositories;

namespace UserService.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection("Infrastructure:Persistence:DatabaseOptions"));

        services.AddSingleton(serviceProvider =>
        {
            string connectionString =
                serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConvertToConnectionString();
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<UserRole>("user_role");
            dataSourceBuilder.MapEnum<UserLoyaltyLevel>("user_loyalty_level");
            return dataSourceBuilder.Build();
        });

        services.AddScoped<IUserRepository, NpgsqlUserRepository>();
        services.AddScoped<IUserLoyaltyAccountRepository, NpgsqlUserLoyaltyAccountRepository>();
        services.AddScoped<IUserLoyaltyPeriodRepository, NpgsqlUserLoyaltyPeriodRepository>();

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider =>
                {
                    string connectionString =
                        serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConvertToConnectionString();
                    return connectionString;
                })
                .WithMigrationsIn(typeof(IMigrationAssemblyMarker).Assembly));
        return services;
    }

    public static async Task RunMigrations(this IServiceProvider provider)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public static async Task StopMigrationByVersion(this IServiceProvider provider, long version)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }
}