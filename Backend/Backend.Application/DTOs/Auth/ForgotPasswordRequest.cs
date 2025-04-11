// Backend.Application/DTOs/Auth/ForgotPasswordRequest.cs

namespace Backend.Application.DTOs.Auth;

public sealed record ForgotPasswordRequest(
    string Email);