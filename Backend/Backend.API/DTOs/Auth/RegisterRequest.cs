using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Auth;

public sealed record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(3)] string Username,
    [Required][MinLength(8)] string Password,
    [Required][MaxLength(50)] string FirstName,
    [Required][MaxLength(50)] string LastName,
    [Required] DateOnly BirthDate,
    [Required] string City,
    [Required] string Country);