using Microsoft.AspNetCore.Identity;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Entities;
using FluentResults;
using Backend.Domain.Errors;
// using Microsoft.AspNetCore.Mvc;

namespace Backend.Infrastructure.Services;

public class SignInService : ISignInService
{
    private readonly SignInManager<UserEntity> _signInManager;

    public SignInService(
        SignInManager<UserEntity> signInManager)
    {
        _signInManager = signInManager;
    }
    
    public async Task<Result<Guid>> LoginAsync(string usernameOrEmail, string password, bool rememberMe)
    {
        var user = usernameOrEmail.Contains('@')
            ? await _signInManager.UserManager.FindByEmailAsync(usernameOrEmail)
            : await _signInManager.UserManager.FindByNameAsync(usernameOrEmail);
        
        
        if (user is null)
            return Result.Fail(DomainErrorFactory.BadRequest("Auth.Invalid", "invalid credentials"));

        var result = await _signInManager.PasswordSignInAsync(
            user, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return Result.Ok(user.Id);

        return result.IsLockedOut
            ? Result.Fail(DomainErrorFactory.Forbidden("Auth.Locked", "account was locked out"))
            : Result.Fail(DomainErrorFactory.BadRequest("Auth.Invalid", "Invalid credentials"));
    }
}