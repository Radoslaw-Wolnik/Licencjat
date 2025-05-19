
namespace Backend.Application.DTOs.Commands.Auth;

public sealed record LoginRequest(
    string UsernameOrEmail,
    string Password,
    bool? RememberMe);