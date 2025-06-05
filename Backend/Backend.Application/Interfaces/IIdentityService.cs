using FluentResults;

namespace Backend.Application.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserWithPasswordAsync(
        Guid id,
        string email, 
        string username, 
        string password, 
        string firstname,
        string lastname,
        string city,
        string country,
        DateOnly birthdate);
}