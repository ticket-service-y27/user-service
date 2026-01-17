using Npgsql;
using UserService.Application.Abstractions.Persistence.Exceptions;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;
using UserService.Application.Models.Users.Operations;

namespace UserService.Infrastructure.Persistence.Repositories;

public class NpgsqlUserLoyaltyAccountRepository : IUserLoyaltyAccountRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlUserLoyaltyAccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreateAsync(long userId, CancellationToken ct)
    {
        const string sql =
            """
            insert into user_loyalty_accounts (user_id)
            values (@user_id)
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id",  userId);

        if (await command.ExecuteNonQueryAsync(ct) != 1)
            throw new CreateEntityException(nameof(UserLoyaltyAccount));
    }

    public async Task<bool> SetBlockedAsync(long userId, bool isBlocked, CancellationToken ct)
    {
        const string sql =
            """
            update user_loyalty_accounts
            set
                is_blocked = @is_blocked,
                blocked_at = @blocked_at
            where user_id = @user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        if (isBlocked)
            command.Parameters.AddWithValue("blocked_at", DateTimeOffset.UtcNow);
        else
            command.Parameters.AddWithValue("blocked_at", DBNull.Value);
        command.Parameters.AddWithValue("is_blocked", isBlocked);
        command.Parameters.AddWithValue("user_id", userId);

        return await command.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<bool> UpdateLoyaltyLevelAsync(
        long userId,
        long periodTotalSpent,
        UserLoyaltyLevel loyaltyLevel,
        DateTimeOffset calculatedAt,
        CancellationToken ct)
    {
        const string sql =
            """
            update user_loyalty_accounts
            set
                period_total_spent = @period_total_spent,
                loyalty_level = @loyalty_level,
                calculated_at = @calculated_at
            where user_id = @user_id and
                  @calculated_at > calculated_at;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("period_total_spent", periodTotalSpent);
        command.Parameters.AddWithValue("loyalty_level", loyaltyLevel);
        command.Parameters.AddWithValue("calculated_at", calculatedAt);
        command.Parameters.AddWithValue("user_id", userId);

        return await command.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<UserLoyaltyState?> GetUserLoyaltyStateAsync(long userId, CancellationToken ct)
    {
        const string sql =
            """
            select loyalty_level, is_blocked
            from user_loyalty_accounts
            where user_id = @user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return new UserLoyaltyState(
            reader.GetFieldValue<UserLoyaltyLevel>(0),
            reader.GetBoolean(1));
    }
}