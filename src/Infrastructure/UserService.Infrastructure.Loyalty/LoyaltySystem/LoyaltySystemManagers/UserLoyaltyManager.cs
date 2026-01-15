using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem.LoyaltySystemManagers;

public class UserLoyaltyManager : IUserLoyaltyManager
{
    private readonly IUserLoyaltyAccountRepository _userLoyaltyAccountRepository;
    private readonly IUserLoyaltyPeriodRepository _userLoyaltyPeriodRepository;
    private readonly ILoyaltyLevelCalculator _loyaltyLevelCalculator;

    public UserLoyaltyManager(
        IUserLoyaltyAccountRepository userLoyaltyAccountRepository,
        IUserLoyaltyPeriodRepository userLoyaltyPeriodRepository,
        ILoyaltyLevelCalculator loyaltyLevelCalculator)
    {
        _userLoyaltyAccountRepository = userLoyaltyAccountRepository;
        _userLoyaltyPeriodRepository = userLoyaltyPeriodRepository;
        _loyaltyLevelCalculator = loyaltyLevelCalculator;
    }

    public async Task<bool> RecalculateAsync(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        UserLoyaltyPeriodState periodState,
        CancellationToken ct)
    {
        long newPeriodStartTotalSpent = periodState.PeriodStartTotalSpent;
        long newPeriodEndTotalSpent = totalSpent;

        long periodTotalSpent = totalSpent - periodState.PeriodStartTotalSpent;
        UserLoyaltyLevel loyaltyLevel = _loyaltyLevelCalculator.CalculateLoyaltyLevel(periodTotalSpent);

        if (totalSpent < periodState.PeriodEndTotalSpent)
        {
            newPeriodStartTotalSpent = totalSpent;
            newPeriodEndTotalSpent = totalSpent;
            periodTotalSpent = 0;
            loyaltyLevel = UserLoyaltyLevel.Bronze;
        }

        bool isPeriodUpdated = await _userLoyaltyPeriodRepository.UpdateAsync(
            userId,
            periodState.PeriodStartAt,
            newPeriodStartTotalSpent,
            newPeriodEndTotalSpent,
            calculatedAt,
            ct);
        if (!isPeriodUpdated)
            return false;

        bool isAccountUpdated = await _userLoyaltyAccountRepository.UpdateLoyaltyLevelAsync(
            userId,
            periodTotalSpent,
            loyaltyLevel,
            calculatedAt,
            ct);
        if (!isAccountUpdated)
            return false;

        return true;
    }
}