using Microsoft.AspNetCore.Identity;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Entities;
using Backend.Application.DTOs.Auth;

using System.Threading.Tasks;
using System.Data;
using AutoMapper;
// using Microsoft.AspNetCore.Mvc;

namespace Backend.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public AuthenticationService(
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _mapper = mapper;
    }
    
    public async Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user is null) return new AuthenticationResult { Succeeded = false };

        var result = await _signInManager.PasswordSignInAsync(
            user, password, rememberMe, lockoutOnFailure: false);

        var mapped = _mapper.Map<AuthenticationResult>(result);
        // mapped.User = _mapper.Map<Domain.User>(user); // Map user if needed
        
        if (!result.Succeeded) 
        {
            mapped.ErrorMessage = result.IsLockedOut 
                ? "Account locked" 
                : "Invalid credentials";
            // also the required 2FA and 
        }

        return mapped;
    }

}
