using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Products.RBA;

public class ProductOwnershipHandler : AuthorizationHandler<ProductOwnershipRequirement, Product>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProductOwnershipRequirement requirement,
        Product product)
    {
        var userName = context.User.FindFirstValue(ClaimsIdentity.DefaultNameClaimType);
        if (userName != null && product.Creator == userName)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}