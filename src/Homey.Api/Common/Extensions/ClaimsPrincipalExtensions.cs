using System.Security.Claims;

namespace Homey.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if(!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
        {
            throw new InvalidOperationException("Invalid UserId");
        }

        return id.ToString();
    }
}