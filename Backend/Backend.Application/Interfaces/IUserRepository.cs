// Application/Interfaces/IUserRepository.cs
using System.Linq.Expressions;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces;
public interface IUserRepository
{
    Task<Result<bool>> ExistsAsync(Expression<Func<User, bool>> predicate);
    Task<Result<User>> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);
    Task<Result<Guid>> AddAsync(User user);
    Task<Result<User>> GetByIdAsync(Guid id);
}