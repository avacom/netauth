using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Services;

namespace Products.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(JwtService jwtService) : ControllerBase
{
    private static readonly Dictionary<string, string> RefreshTokens = new();
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != "admin" || request.Password != "abc123!")
            return Unauthorized("Invalid credentials");

        var accessToken = jwtService.GenerateAccessToken(request.Username);
        var refreshToken = jwtService.GenerateRefreshToken();

        RefreshTokens[request.Username] = refreshToken;

        return Ok(new
        {
            access_token = accessToken,
            refresh_token = refreshToken
        });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        if (!RefreshTokens.ContainsValue(request.RefreshToken))
            return Unauthorized("Invalid refresh token");

        var username = RefreshTokens.FirstOrDefault(x => x.Value == request.RefreshToken).Key;
        if (username is null)
            return Unauthorized();

        // rotate token
        var newAccessToken = jwtService.GenerateAccessToken(username);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        RefreshTokens[username] = newRefreshToken;

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var username = User.Identity?.Name;
        var department = User.Claims.FirstOrDefault(c => c.Type == "department")?.Value;
        var company = User.Claims.FirstOrDefault(c => c.Type == "company")?.Value;
        var birthDate = User.Claims.FirstOrDefault(c => c.Type == "birthDate")?.Value;
        return Ok(new { username, department, company, birthDate });
    }
}

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);