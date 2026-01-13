namespace UserService.Infrastructure.Security.JwtToken;

public class JwtAuthenticationOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SigningKey { get; set; } = string.Empty;

    public int LifetimeSeconds { get; set; }
}
