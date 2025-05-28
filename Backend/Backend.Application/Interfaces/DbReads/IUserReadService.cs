using System.Linq.Expressions;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Common;
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.DbReads;

public interface IUserReadService
{
    Task<bool> ExistsAsync(Expression<Func<UserProjection, bool>> predicate, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid UserId, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithIncludes(Guid userId,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserProjection, object>>[] includes);
    // should be changed to get full user by id
    // or mby we dont even need this function

    Task<SocialMediaLink> GetSocialMediaByIdAsync(Guid SocialMediaId, CancellationToken cancellationToken = default); // only for updating a single social Media
}