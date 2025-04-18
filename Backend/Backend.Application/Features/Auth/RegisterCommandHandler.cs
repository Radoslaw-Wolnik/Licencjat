
// Application/Features/Auth/RegisterCommandHandler.cs
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Features.Auth;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepo;
    private readonly IIdentityService _identityService;

    // constructor
    public RegisterCommandHandler(
        IUserRepository userRepository,
        IIdentityService identityService) {
            _userRepo = userRepository;
            _identityService = identityService;
        }

    public async Task<Result<Guid>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {

        Console.WriteLine($"[Register command handler] Succesfully passed to the register command handler");
        Console.WriteLine($"Email passed: {command.Email}");

        // 1. Check uniqueness via repository
        var emailExists = await _userRepo.ExistsAsync(u => u.Email == command.Email);
        if (emailExists.Value) return Result.Fail<Guid>(AuthErrors.EmailAlreadyExists);

        Console.WriteLine($"[Register command handler] checked the email for colisions");

        var usernameExists = await _userRepo.ExistsAsync(u => u.Username == command.Username);
        if (usernameExists.Value) return Result.Fail<Guid>(AuthErrors.UsernameTaken);

        Console.WriteLine($"[Register command handler] checked the username for colisions");
        

        // 2. Create domain entity

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

        // 3. Create identity user (password handled here)
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

        // 4. Persist domain user
        await _userRepo.AddAsync(userResult.Value);
        Console.WriteLine($"[Register command handler] repo added");
        return userResult.Value.Id;
    }
}