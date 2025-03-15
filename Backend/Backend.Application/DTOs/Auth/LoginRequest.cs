// Backend.Application/DTOs/Auth/LoginRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = false;
}
