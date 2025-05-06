using Backend.Application.Interfaces;
using Backend.Application.Interfaces.DbReads;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Auth;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IWriteUserRepository _userWrite;
    private readonly IUserReadService _userRead;
    private readonly IIdentityService _identityService;

    // constructor
    public RegisterCommandHandler(
        IWriteUserRepository writeUserRepository,
        IUserReadService userReadService,
        IIdentityService identityService) {
            _userWrite = writeUserRepository;
            _userRead = userReadService;
            _identityService = identityService;
        }

    public async Task<Result<Guid>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {

        Console.WriteLine($"[Register command handler] Succesfully passed to the register command handler");
        Console.WriteLine($"Email passed: {command.Email}");

        // Check uniqueness
        var emailExists = await _userRead.ExistsAsync(u => u.Email == command.Email);
        if (emailExists) return Result.Fail<Guid>(AuthErrors.EmailAlreadyExists);

        Console.WriteLine($"[Register command handler] checked the email for colisions");

        var usernameExists = await _userRead.ExistsAsync(u => u.Username == command.Username);
        if (usernameExists) return Result.Fail<Guid>(AuthErrors.UsernameTaken);

        Console.WriteLine($"[Register command handler] checked the username for colisions");
        

        // Create domain entity

        var code = CountryCode.Create(command.Country);
        if (code.IsFailed) return Result.Fail<Guid>(AuthErrors.InvalidCountryCode);
        

        var loc = Location.Create(city: command.City, country: code.Value);
        if (loc.IsFailed) return Result.Fail<Guid>(LocationErrors.WrongLocation);

        var userResult = User.Create(
            command.Email,
            command.Username,
            command.FirstName,
            command.LastName,
            command.BirthDate,
            loc.Value);
        
        if (userResult.IsFailed) return Result.Fail<Guid>(userResult.Errors);
        Console.WriteLine($"[Register command handler] Created domain entity");

        // Create identity user (password handled here)
        var identityResult = await _identityService.CreateUserWithPasswordAsync(
            command.Email,
            command.Username,
            command.Password,
            command.FirstName,
            command.LastName,
            command.City,
            command.Country,
            command.BirthDate);

        if (identityResult.IsFailed)
            return Result.Fail<Guid>(identityResult.Errors);
        
        Console.WriteLine($"[Register command handler] Created identity service response success");

        // Persist domain user
        await _userWrite.AddAsync(userResult.Value);
        Console.WriteLine($"[Register command handler] repo added");
        return userResult.Value.Id;
    }
}