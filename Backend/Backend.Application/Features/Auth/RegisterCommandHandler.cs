
// Application/Features/Auth/RegisterCommandHandler.cs
using Backend.Application.Interfaces;
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
        // 1. Check uniqueness via repository
        var emailExists = await _userRepo.ExistsAsync(u => u.Email == command.Email);
        if (emailExists.Value) return Result.Fail<Guid>(UserErrors.EmailAlreadyExists);

        var usernameExists = await _userRepo.ExistsAsync(u => u.Username == command.Username);
        if (usernameExists.Value) return Result.Fail<Guid>(UserErrors.UsernameTaken);

        // 2. Create domain entity
        var userResult = User.Create(
            command.Email,
            command.Username,
            command.FirstName,
            command.LastName,
            command.BirthDate);
        
        if (userResult.IsFailed) return Result.Fail<Guid>(userResult.Errors);

        // 3. Create identity user (password handled here)
        var identityResult = await _identityService.CreateUserWithPasswordAsync(
            command.Email,
            command.Username,
            command.Password,
            command.FirstName,
            command.LastName,
            command.BirthDate);

        if (identityResult.IsFailed)
            return Result.Fail<Guid>(identityResult.Errors);

        // 4. Persist domain user
        await _userRepo.AddAsync(userResult.Value);
        return userResult.Value.Id;
    }
}