
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

        var loginInfoResult = await _userRepo.GetLoginInfoAsync(predicate);
        if (loginInfoResult.IsFailed)
            return Result.Fail<User>(AuthErrors.InvalidCredentials);

        // pass the hash into your sign-in service
        var signInResult = await _signInService.LoginAsync(
            loginInfoResult.Value, command.Password, command.RememberMe ?? false);

        if (!signInResult.IsSuccess)
            return Result.Fail<User>(AuthErrors.InvalidCredentials);
        
        Console.WriteLine($"\n\n[Login command handler] got all the way down to the user fetching\n\n");

        // now that they’re authenticated, load the full domain user:
        var userResult = await _userRepo.GetByAsync(predicate);
        return userResult;
    }
}