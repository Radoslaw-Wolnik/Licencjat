using System.Security.Claims;

namespace Backend.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim not found");

        // if (userIdClaim == null)
        //    return null;

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            throw new InvalidOperationException("Invalid User ID format");

        return userId;
    }

    // Optional: Nullable version for non-authenticated users
    public static Guid? GetUserIdOrNull(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim?.Value, out var userId) 
            ? userId 
            : null;
    }
}