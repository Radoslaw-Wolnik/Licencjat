using System.Security.Claims;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Backend.Infrastructure.Services;

public sealed class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpUserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid? UserId =>
        _contextAccessor.HttpContext?.User.GetUserIdOrNull(); // this one doesnt throw error if the user is not uathenticated - safer aproach

    public Guid GetRequiredUserId() =>
        _contextAccessor.HttpContext?.User.GetUserId()
        ?? throw new InvalidOperationException("No HTTP context or user not authenticated");


    public string? Username =>
        _contextAccessor.HttpContext?.User.Identity?.Name;

    public bool IsAuthenticated =>
        _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool HasClaim(string type, string value) =>
        _contextAccessor.HttpContext?
            .User.HasClaim(type, value) ?? false;

    public IEnumerable<Claim> GetClaims() =>
        _contextAccessor.HttpContext?.User.Claims ?? Enumerable.Empty<Claim>();
    
    public bool IsInRole(string role) =>
        _contextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}

// In controllers:
// var userId = User.GetUserId();