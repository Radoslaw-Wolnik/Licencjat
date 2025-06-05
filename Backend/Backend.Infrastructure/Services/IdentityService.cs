using Backend.Application.Interfaces;
using Backend.Domain.Errors;
using Backend.Infrastructure.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<UserEntity> _userManager;

    public IdentityService(
        UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<Guid>> CreateUserWithPasswordAsync(
        Guid id,
        string email, 
        string username, 
        string password,
        string firstname,
        string lastname,
        string city,
        string country,
        DateOnly birthdate)
    {
        var user = new UserEntity { Id = id, Email = email, UserName = username, FirstName = firstname, LastName = lastname, City = city, Country = country, BirthDate = birthdate, Reputation = 4.0f};
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return Result.Ok(user.Id);

        // Convert IdentityErrors to domain errors
        var errors = result.Errors.Select(e => 
            e.Code switch
            {
                "DuplicateUserName" => DomainErrorFactory.AlreadyExists("User.UsernameExists", e.Description),
                "DuplicateEmail" => DomainErrorFactory.AlreadyExists("User.EmailExists", e.Description),
                "PasswordTooShort" => DomainErrorFactory.Invalid("User.PasswordTooShort", e.Description),
                _ => DomainErrorFactory.Invalid(e.Code, e.Description)
            });
        
        return Result.Fail(errors);
    }
}