using Backend.Application.DTOs.Auth;

namespace Backend.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe);
}
