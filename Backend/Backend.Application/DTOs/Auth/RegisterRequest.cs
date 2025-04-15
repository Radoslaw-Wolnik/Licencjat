// Backend.Application/DTOs/Auth/RegisterRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string City,
    string Country);