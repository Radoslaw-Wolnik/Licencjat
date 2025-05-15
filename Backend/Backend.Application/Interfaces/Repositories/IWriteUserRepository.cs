using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserRepository
{
    Task<Result<Guid>> AddAsync(User user, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(User user, CancellationToken cancellationToken, params Expression<Func<UserProjection, object>>[] includes);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}