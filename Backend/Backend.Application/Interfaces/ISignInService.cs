using Backend.Application.DTOs.Commands.Auth;
using FluentResults;

namespace Backend.Application.Interfaces;

public interface ISignInService
{
    Task<Result> LoginAsync(LoginUserInfo userInfo, string usernameOrEmail, bool rememberMe);
}
