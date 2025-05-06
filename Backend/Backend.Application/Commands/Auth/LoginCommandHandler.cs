using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;

namespace Backend.Application.Commands.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<User>>
{
    private readonly IAuthUserRepository _authRepo;
    private readonly IUserReadService _userRead;
    private readonly ISignInService _signInService;

    // constructor ?
    public LoginCommandHandler(
        IAuthUserRepository authUserRepository,
        IUserReadService userReadService,
        ISignInService signInService) {
            _authRepo = authUserRepository;
            _userRead = userReadService;
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

        var loginInfo = await _authRepo.GetLoginInfoAsync(predicate);
        if (loginInfo == null)
            return Result.Fail<User>(AuthErrors.InvalidCredentials);

        // pass the hash into your sign-in service
        var signInResult = await _signInService.LoginAsync(
            loginInfo, command.Password, command.RememberMe ?? false);

        if (!signInResult.IsSuccess)
            return Result.Fail<User>(AuthErrors.InvalidCredentials);
        
        Console.WriteLine($"\n\n[Login command handler] got all the way down to the user fetching\n\n");

        // now that they’re authenticated, load the full domain user:
        var user = await _userRead.GetByAsync(predicate);
        if (user == null)
            return Result.Fail<User>(UserErrors.NotFound);
        
        return user;
    }
}