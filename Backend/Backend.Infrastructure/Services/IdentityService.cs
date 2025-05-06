using Backend.Application.Interfaces;
using Backend.Domain.Common;
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
    
    public async Task<Result> CreateUserWithPasswordAsync(
        string email, 
        string username, 
        string password,
        string firstname,
        string lastname,
        string city,
        string country,
        DateOnly birthdate)
    {
        var user = new UserEntity { Email = email, UserName = username, FirstName = firstname, LastName = lastname, City = city, Country = country, BirthDate = birthdate};
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? Result.Ok()
            : Result.Fail(result.Errors
                .Select(e => new DomainError(e.Code, e.Description, ErrorType.Validation)));
    }
}