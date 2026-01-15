namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

public class LoyaltyLevelCalculatorOptions
{
    public IReadOnlyList<LoyaltyLevelOptions> Levels { get; set; } = [];

    public LoyaltyBackgroundServiceOptions Period { get; set; } = new();
}