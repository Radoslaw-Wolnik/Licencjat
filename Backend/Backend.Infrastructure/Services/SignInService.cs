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
    private readonly UserManager<UserEntity> _userManager;

    public SignInService(
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result<Guid>> LoginAsync(string usernameOrEmail, string password, bool rememberMe)
    {
        // Handle both email and username login
        var user = usernameOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(usernameOrEmail)
            : await _userManager.FindByNameAsync(usernameOrEmail);

        if (user is null)
            return Result.Fail(DomainErrorFactory.Unauthorized("Auth.InvalidCredentials", "Invalid username or password"));

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user))
            return Result.Fail(DomainErrorFactory.Forbidden("Auth.AccountLocked", "Account is temporarily locked"));

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, password, lockoutOnFailure: true);

        if (result.Succeeded)
            return Result.Ok(user.Id);

        // Handle specific error cases
        return result.IsLockedOut
            ? Result.Fail(DomainErrorFactory.Forbidden("Auth.AccountLocked", "Too many failed attempts"))
            : Result.Fail(DomainErrorFactory.Unauthorized("Auth.InvalidCredentials", "Invalid password"));
    }
}