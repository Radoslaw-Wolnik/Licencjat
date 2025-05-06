using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Commands.Auth;

namespace Backend.Application.Interfaces.Repositories;

public interface IAuthUserRepository // change to read service
{
    Task<LoginUserInfo?> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate);
}