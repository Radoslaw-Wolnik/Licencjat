// Backend.Application/DTOs/Auth/ForgotPasswordRequest.cs

namespace Backend.Application.DTOs.Commands.Auth;

public sealed record ForgotPasswordRequest(
    string Email);