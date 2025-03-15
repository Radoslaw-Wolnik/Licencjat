// Backend.Application/DTOs/Auth/ForgotPasswordRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = null!;
}