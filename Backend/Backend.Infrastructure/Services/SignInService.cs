using Microsoft.AspNetCore.Identity;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Entities;
using Backend.Application.DTOs.Auth;

using System.Threading.Tasks;
using System.Data;
using AutoMapper;
using FluentResults;
using Backend.Domain.Errors;
// using Microsoft.AspNetCore.Mvc;

namespace Backend.Infrastructure.Services;

public class SignInService : ISignInService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;

    public SignInService(
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _mapper = mapper;
    }
    
    public async Task<Result> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(UserErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(
            user, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return Result.Ok();

        return result.IsLockedOut
            ? Result.Fail(UserErrors.AccountLocked)
            : Result.Fail(UserErrors.InvalidCredentials);
    }
}