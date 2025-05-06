namespace Backend.Application.DTOs.Commands.Auth;

public record LoginResponse(
    Guid UserId,
    string Username);


/*
namespace Backend.Application.DTOs.Auth;

public class AuthenticationResult // public record AuthenticationResult insted of class mby?
{
    public bool Succeeded { get; init; } - we dont need that one as i use code 200 for success and other as fail
    public bool RequiresTwoFactor { get; init; }
    public bool IsLockedOut { get; init; }
    public string? ErrorMessage { get; set; }
}
*/