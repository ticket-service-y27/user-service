namespace UserService.Application.Abstractions.LoyaltySystem.Managers;

public interface ILoyaltyPeriodManager
{
    Task MoveExpiredPeriodsAsync(DateTimeOffset timeNow, TimeSpan periodLength, CancellationToken ct);
}