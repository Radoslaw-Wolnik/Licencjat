namespace Backend.Application.DTOs.Auth;

public class AuthenticationResult // public record AuthenticationResult insted of class mby?
{
    public bool Succeeded { get; init; }
    public bool RequiresTwoFactor { get; init; }
    public bool IsLockedOut { get; init; }
    public string? ErrorMessage { get; set; }
}