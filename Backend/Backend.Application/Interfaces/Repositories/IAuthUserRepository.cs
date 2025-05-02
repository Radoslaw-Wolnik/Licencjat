using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;

namespace Backend.Application.Interfaces.Repositories;

public interface IAuthUserRepository
{
    Task<LoginUserInfo?> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate);
}