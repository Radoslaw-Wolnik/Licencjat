// Backend.Application/Services/UserService.cs
using Backend.Domain.Entities;
using Backend.Application.Interfaces;


namespace Backend.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    
    public UserService(IUserRepository userRepository, IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }
    
    public async Task RegisterUserAsync(string email, string username, string password, string firstName, string lastName, DateTime birthDate)
    {
        // You might have additional business logic here (e.g., checking for duplicate emails)
        var user = new User(email, username, firstName, lastName, birthDate);
        await _userRepository.AddAsync(user, password);
    }

    public async Task LoginUserAsync(string email, string password, bool rememberMe)
    {
        var signInResult = await _authenticationService.LoginAsync(email, password, rememberMe);
        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        // Optionally perform additional business logic after successful login
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}

