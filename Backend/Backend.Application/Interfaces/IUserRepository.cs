// Application/Interfaces/IUserRepository.cs
using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces;
public interface IUserRepository
{
    Task<Result<bool>> ExistsAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<Result<User>> GetByAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<Result<Guid>> AddAsync(User user);
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<LoginUserInfo>> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate);
}