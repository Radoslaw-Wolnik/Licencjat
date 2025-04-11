// Backend.Application/DTOs/Auth/LoginRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public sealed record LoginRequest(
    string UsernameOrEmail,
    string Password,
    bool? RememberMe);