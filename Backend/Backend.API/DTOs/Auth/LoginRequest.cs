using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Auth;

public sealed record LoginRequest(
    [Required] string UsernameOrEmail,
    [Required] string Password,
    bool? RememberMe);
