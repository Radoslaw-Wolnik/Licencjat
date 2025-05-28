using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Auth;

public sealed record ForgotPasswordRequest(
    [Required][EmailAddress] string Email);