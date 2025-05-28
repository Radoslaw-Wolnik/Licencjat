namespace Backend.API.DTOs.Auth;

public sealed record LoginResponse(
    Guid UserId,
    string Username);