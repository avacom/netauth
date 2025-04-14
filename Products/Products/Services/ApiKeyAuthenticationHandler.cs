using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Products.Services;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string HeaderName = "X-API-Key";
    private readonly Dictionary<string, string?> _apiKeys = new()
    {
        { "apikey1", "SalesForce" },
        { "apikey2", "SmartSheet" }
    };
    
    [Obsolete("Obsolete")]
    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API key was not provided"));
        }

        var app = GetApp(extractedApiKey);
        
        if (app == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("API key was invalid"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, app),
            new Claim(ClaimTypes.Role, "Application")
        };
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    
    private string? GetApp(string apiKey)
    {
        return _apiKeys.GetValueOrDefault(apiKey);
    }
}