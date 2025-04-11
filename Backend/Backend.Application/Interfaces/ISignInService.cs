using Backend.Application.DTOs.Auth;
using FluentResults;

namespace Backend.Application.Interfaces;

public interface ISignInService
{
    Task<Result> LoginAsync(string password, string usernameOrEmail, bool rememberMe);
}
