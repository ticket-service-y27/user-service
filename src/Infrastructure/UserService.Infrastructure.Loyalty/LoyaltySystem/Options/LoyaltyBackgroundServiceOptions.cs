namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

public class LoyaltyBackgroundServiceOptions
{
    public int LengthDays { get; init; }

    public int CheckEveryMinutes { get; init; }

    public int BatchSize { get; init; }
}