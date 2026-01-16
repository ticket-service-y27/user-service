using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.LoyaltySystem.Managers;
using UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem;

public class LoyaltyPeriodBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<LoyaltyBackgroundServiceOptions> _optionsMonitor;

    public LoyaltyPeriodBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<LoyaltyBackgroundServiceOptions> options)
    {
        _scopeFactory = scopeFactory;
        _optionsMonitor = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(_optionsMonitor.CurrentValue.CheckEverySeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            ILoyaltyPeriodManager periodManager = scope.ServiceProvider.GetRequiredService<ILoyaltyPeriodManager>();

            await periodManager.MoveExpiredPeriodsAsync(
                DateTimeOffset.UtcNow,
                periodLength: TimeSpan.FromSeconds(_optionsMonitor.CurrentValue.PeriodLengthSeconds),
                stoppingToken);
        }
    }
}