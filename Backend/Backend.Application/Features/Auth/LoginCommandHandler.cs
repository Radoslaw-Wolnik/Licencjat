
// Application/Features/Auth/RegisterCommandHandler.cs
using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<User>>
{
    private readonly IUserRepository _userRepo;
    private readonly ISignInService _signInService;

    // constructor ?
    public LoginCommandHandler(
        IUserRepository userRepository,
        ISignInService signInService) {
            _userRepo = userRepository;
            _signInService = signInService;
        }

    public async Task<Result<User>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // Determine if input is email or username
        var isEmail = command.UsernameOrEmail.Contains('@');
        Console.WriteLine($"[Login command handler] checked the email and is {isEmail}");
        
        Expression<Func<UserProjection, bool>> predicate = isEmail
            ? u => u.Email == command.UsernameOrEmail
            : u => u.Username == command.UsernameOrEmail;

        var userResult = await _userRepo.FirstOrDefaultAsync(predicate);
        Console.WriteLine($"[Login command handler] user result: {userResult}");
        
        if (userResult.IsFailed){
            return Result.Fail<User>(AuthErrors.InvalidCredentials);
        }

        // Attempt login with stored email
        var loginResult = await _signInService.LoginAsync(
            userResult.Value.Email,
            command.Password,
            command.RememberMe ?? false);

        return loginResult.IsSuccess 
            ? userResult
            : Result.Fail(loginResult.Errors);
    }
}