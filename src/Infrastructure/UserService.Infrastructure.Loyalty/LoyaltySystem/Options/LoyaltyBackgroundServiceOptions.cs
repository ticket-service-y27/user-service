namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

public class LoyaltyBackgroundServiceOptions
{
    public long PeriodLenghtSeconds { get; set; }

    public int CheckEverySeconds { get; set; }
}