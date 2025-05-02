using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapIssueRepository
{
    Task<Issue> GetByIdAsync(Guid subSwapId);

    Task AddAsync(Issue issue);
    Task RemoveAsync(Guid issueId);
    Task UpdateAsync(Issue issue);
}