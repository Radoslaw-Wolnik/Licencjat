using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;

namespace Backend.Application.Commands.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<User>>
{
    private readonly IUserReadService _userRead;
    private readonly ISignInService _signInService;

    // constructor ?
    public LoginCommandHandler(
        IUserReadService userReadService,
        ISignInService signInService) {
            _userRead = userReadService;
            _signInService = signInService;
        }

    public async Task<Result<User>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // pass the hash into your sign-in service
        var signInResult = await _signInService.LoginAsync(
            command.UsernameOrEmail, command.Password, command.RememberMe ?? false);

        if (!signInResult.IsSuccess)
            return Result.Fail<User>(DomainErrorFactory.BadRequest("Auth.Invalid", "Invalid credentials"));
        
        Console.WriteLine($"\n\n[Login command handler] got all the way down to the user fetching\n\n");

        // now that they’re authenticated, load the full domain user:
        var user = await _userRead.GetByIdAsync(signInResult.Value, cancellationToken);
        if (user == null)
            return Result.Fail<User>(DomainErrorFactory.NotFound("User", signInResult.Value));
        
        return user;
    }
}