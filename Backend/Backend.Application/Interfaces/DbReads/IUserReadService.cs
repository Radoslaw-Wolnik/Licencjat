using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Commands.Auth;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserReadService
{
    Task<bool> ExistsAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<User?> GetByIdAsync(Guid UserId);
    Task<User?> GetByAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<User?> GetUserWithIncludes(Guid userId, 
        params Expression<Func<UserProjection, object>>[] includes);

    Task<LoginUserInfo?> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate);
}