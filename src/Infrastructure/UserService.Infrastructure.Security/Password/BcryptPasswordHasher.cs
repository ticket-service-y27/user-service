using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Security;
using UserService.Application.Exceptions;

namespace UserService.Infrastructure.Security.Password;

public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasherOptions _options;

    public BcryptPasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _options = options.Value;
    }

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: _options.BcryptWorkFactor);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}