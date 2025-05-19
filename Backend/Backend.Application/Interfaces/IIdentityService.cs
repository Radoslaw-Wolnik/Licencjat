using FluentResults;

namespace Backend.Application.Interfaces;

public interface IIdentityService
{
    Task<Result> CreateUserWithPasswordAsync(
        string email, 
        string username, 
        string password, 
        string firstname,
        string lastname,
        string city,
        string country,
        DateOnly birthdate);
}