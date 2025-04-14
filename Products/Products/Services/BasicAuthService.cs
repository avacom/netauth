using AspNetCore.Authentication.Basic;

namespace Products.Services;

public class BasicAuthService : IBasicUserValidationService
{
    public Task<bool> IsValidAsync(string username, string password)
    {
        return Task.FromResult(username == "admin" && password == "abc123!");
    }
}