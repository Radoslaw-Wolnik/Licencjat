using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapIssueRepository
{
    Task<Result<Guid>> AddAsync(Issue issue, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid issueId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Issue issue, CancellationToken cancellationToken);
}