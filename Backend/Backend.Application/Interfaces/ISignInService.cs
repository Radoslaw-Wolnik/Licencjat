using FluentResults;

namespace Backend.Application.Interfaces;

public interface ISignInService
{
    Task<Result<Guid>> LoginAsync(string usernameOrEmail, string password, bool rememberMe);
}
