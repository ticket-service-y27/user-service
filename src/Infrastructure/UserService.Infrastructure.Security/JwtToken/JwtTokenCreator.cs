using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Abstractions.Security;
using UserService.Application.Models.Users.Enums;

namespace UserService.Infrastructure.Security.JwtToken;

public class JwtTokenCreator : IJwtTokenCreator
{
    private readonly JwtAuthenticationOptions _options;

    public JwtTokenCreator(IOptions<JwtAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public string CreateAccessToken(long userId, UserRole role)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey);
        var symmetricKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(
            symmetricKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("sub", userId.ToString()),
            new Claim("role", MapUserRole(role)),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_options.LifetimeSeconds),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string MapUserRole(UserRole role)
    {
        return role switch
        {
            UserRole.User => "user",
            UserRole.Admin => "admin",
            UserRole.Organizer => "organizer",
            _ => throw new ArgumentOutOfRangeException(
                nameof(role), role, $"Exception. User role {role} is not supported"),
        };
    }
}