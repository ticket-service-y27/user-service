namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

public class LoyaltyBackgroundServiceOptions
{
    public long PeriodLengthSeconds { get; set; }

    public int CheckEverySeconds { get; set; }
}