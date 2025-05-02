using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface IUserSocialMediaRepository
{
    Task<IReadOnlyCollection<SocialMediaLink>> GetByUserIdAsync(Guid userId);

    Task AddAsync(SocialMediaLink link);
    Task RemoveAsync(Guid linkId);
    Task UpdateAsync(SocialMediaLink link);
}