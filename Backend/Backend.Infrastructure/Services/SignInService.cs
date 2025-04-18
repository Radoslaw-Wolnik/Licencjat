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
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly IMapper _mapper;

    public SignInService(
        SignInManager<UserEntity> signInManager,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _mapper = mapper;
    }
    
    public async Task<Result> LoginAsync(LoginUserInfo userInfo, string password, bool rememberMe)
    {
        var email = userInfo.Email;
        if (email is null)
            return Result.Fail(AuthErrors.InvalidCredentials); // its like impossible

        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(AuthErrors.InvalidCredentials);

        var result = await _signInManager.PasswordSignInAsync(
            user, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return Result.Ok();

        return result.IsLockedOut
            ? Result.Fail(AuthErrors.AccountLocked)
            : Result.Fail(AuthErrors.InvalidCredentials);
    }
}