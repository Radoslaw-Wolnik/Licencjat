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


        var userId = Guid.NewGuid();

        // Create domain entity

        var code = CountryCode.Create(command.Country);
        if (code.IsFailed) return Result.Fail<Guid>(DomainErrorFactory.Invalid("CountryCode", "Country with this code is not registered"));
        

        var loc = Location.Create(city: command.City, country: code.Value);
        if (loc.IsFailed) return Result.Fail<Guid>(DomainErrorFactory.Invalid("Location", "Given Location is invalid"));

        var userResult = User.Create(
            userId,
            command.Email,
            command.Username,
            command.FirstName,
            command.LastName,
            command.BirthDate,
            loc.Value);
        
        if (userResult.IsFailed) return Result.Fail<Guid>(userResult.Errors);
        Console.WriteLine($"[Register command handler] Created domain entity");

        // Check uniqueness
        // Create identity user (password handled here)
        var identityResult = await _identityService.CreateUserWithPasswordAsync(
            userId,
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
        var saveResult = await _userWrite.AddAsync(userResult.Value, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);
        
        Console.WriteLine($"[Register command handler] repo added");
        return userResult.Value.Id;
    }
}