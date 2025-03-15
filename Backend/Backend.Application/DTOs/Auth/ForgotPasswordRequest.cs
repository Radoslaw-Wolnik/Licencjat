// Backend.Application/DTOs/Auth/ForgotPasswordRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

}