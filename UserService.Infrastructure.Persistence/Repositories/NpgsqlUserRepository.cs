using Npgsql;
using UserService.Application.Abstractions.Persistence.Exceptions;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users;

namespace UserService.Infrastructure.Persistence.Repositories;

public class NpgsqlUserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlUserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(
        string nickname,
        string email,
        string passwordHash,
        UserRole role,
        CancellationToken ct)
    {
        const string sql =
            """
            insert into users (user_nickname, user_email, user_password_hash, user_role, user_created_at) 
            values (@nickname, @email, @password_hash, @role, @created_at)
            returning user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("nickname", nickname);
        command.Parameters.AddWithValue("email",  email);
        command.Parameters.AddWithValue("password_hash",  passwordHash);
        command.Parameters.AddWithValue("role", role);
        command.Parameters.AddWithValue("created_at", DateTimeOffset.UtcNow);

        object? id = await command.ExecuteScalarAsync(ct);
        if (id is null)
            throw new CreateEntityException(nameof(User));

        return (long)id;
    }

    public async Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct)
    {
        const string sql =
            """
            update users
            set user_role = @role
            where user_id = @user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("role", role);
        command.Parameters.AddWithValue("user_id", userId);

        if (await command.ExecuteNonQueryAsync(ct) == 0)
            throw new NotFoundException(nameof(userId), userId.ToString());
    }

    public async Task BlockUserByIdAsync(long userId, CancellationToken ct)
    {
        const string sql =
            """
            update users
            set 
                user_is_blocked = true,
                user_blocked_at = @blocked_at
            where user_id = @user_id;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("blocked_at", DateTimeOffset.UtcNow);
        command.Parameters.AddWithValue("user_id", userId);

        if (await command.ExecuteNonQueryAsync(ct) == 0)
            throw new NotFoundException(nameof(userId), userId.ToString());
    }

    public async Task<User?> GetUserByNicknameAsync(string nickname, CancellationToken ct)
    {
        const string sql =
            """
            select 
                user_id, 
                user_nickname, 
                user_email, 
                user_password_hash, 
                user_role, 
                user_created_at
            from users
            where user_nickname = @nickname;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("nickname", nickname);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return new User(
            Id: reader.GetInt64(0),
            Nickname: reader.GetString(1),
            Email: reader.GetString(2),
            PasswordHash: reader.GetString(3),
            Role: reader.GetFieldValue<UserRole>(4),
            CreatedAt: reader.GetFieldValue<DateTimeOffset>(5));
    }
}