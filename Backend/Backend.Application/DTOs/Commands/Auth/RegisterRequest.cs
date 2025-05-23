
namespace Backend.Application.DTOs.Commands.Auth;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string City,
    string Country);