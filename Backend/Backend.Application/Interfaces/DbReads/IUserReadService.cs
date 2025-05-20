using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Commands.Auth;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserReadService
{
    Task<bool> ExistsAsync(Expression<Func<UserProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid UserId, CancellationToken cancellationToken = default);
    Task<User?> GetByAsync(Expression<Func<UserProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithIncludes(Guid userId,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserProjection, object>>[] includes);

    Task<LoginUserInfo?> GetLoginInfoAsync(Expression<Func<UserProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<SocialMediaLink> GetSocialMediaByIdAsync(Guid SocialMediaId, CancellationToken cancellationToken = default); // only for updating a single social Media
}