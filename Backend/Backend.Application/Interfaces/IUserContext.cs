using System.Security.Claims;

namespace Backend.Application.Interfaces;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }

    // Optional for advanced claims
    bool HasClaim(string type, string value);
    IEnumerable<Claim> GetClaims();
    bool IsInRole(string role);
}