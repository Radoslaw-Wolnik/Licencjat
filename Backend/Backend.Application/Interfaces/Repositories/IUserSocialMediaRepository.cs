using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserSocialMediaRepository
{
    Task<Result<Guid>> AddAsync(SocialMediaLink link, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid linkId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(SocialMediaLink link, CancellationToken cancellationToken);
}