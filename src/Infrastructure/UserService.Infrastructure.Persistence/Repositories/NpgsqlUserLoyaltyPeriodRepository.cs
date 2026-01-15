using Npgsql;
using UserService.Application.Abstractions.Persistence.Exceptions;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users;

namespace UserService.Infrastructure.Persistence.Repositories;

public class NpgsqlUserLoyaltyPeriodRepository : IUserLoyaltyPeriodRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlUserLoyaltyPeriodRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreateAsync(long userId, CancellationToken ct)
    {
        const string sql =
            """
            insert into user_loyalty_periods (user_id)
            values (@user_id)
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id",  userId);

        if (await command.ExecuteNonQueryAsync(ct) != 1)
            throw new CreateEntityException(nameof(UserLoyaltyPeriodState));
    }

    public async Task<bool> UpdateAsync(
        long userId,
        DateTimeOffset periodStartAt,
        long periodStartTotalSpent,
        long periodEndTotalSpent,
        DateTimeOffset calculatedAt,
        CancellationToken ct)
    {
        const string sql =
            """
            update user_loyalty_periods
            set
                period_start_at = @period_start_at,
                period_start_total_spent = @period_start_total_spent,
                period_end_total_spent = @period_end_total_spent,
                calculated_at = @calculated_at
            where user_id = @user_id and
                  @calculated_at > calculated_at;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("period_start_at",  periodStartAt);
        command.Parameters.AddWithValue("period_start_total_spent", periodStartTotalSpent);
        command.Parameters.AddWithValue("period_end_total_spent", periodEndTotalSpent);
        command.Parameters.AddWithValue("calculated_at", calculatedAt);
        command.Parameters.AddWithValue("user_id", userId);

        return await command.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<UserLoyaltyPeriodState?> GetAsync(long userId, CancellationToken ct)
    {
        const string sql =
            """
            select
                user_id,
                period_start_at,
                period_start_total_spent,
                period_end_total_spent,
                calculated_at
            from user_loyalty_periods
            where user_id = @user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return new UserLoyaltyPeriodState(
            UserId: reader.GetInt64(0),
            PeriodStartAt: reader.GetFieldValue<DateTimeOffset>(1),
            PeriodStartTotalSpent: reader.GetInt64(2),
            PeriodEndTotalSpent: reader.GetInt64(3),
            CalculatedAt: reader.GetFieldValue<DateTimeOffset>(4));
    }
}