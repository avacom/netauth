using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Products.Services;

public class JwtService
{
    public const string SecretKey = "12345678901234567890123456789012";
    public const string Issuer = "MyIssuer";
    public const string Audience = "MyAudience";
    private const int AccessTokenExpirationMinutes = 5;

    public string GenerateAccessToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Software Engineer"),
            new Claim("department", "EPD"),
            new Claim("company", "Grammarly"),
            new Claim("birthDate", "1987-09-27"),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken() => Guid.NewGuid().ToString();
}