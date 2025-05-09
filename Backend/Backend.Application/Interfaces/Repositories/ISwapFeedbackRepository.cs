using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapFeedbackRepository
{
    Task<Feedback> GetByIdAsync(Guid swapId);

    Task<Result<Guid>> AddAsync(Feedback feedback, CancellationToken cancellationToken);
    Task<Result> RemoveAsync(Guid feedbackId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Feedback feedback, CancellationToken cancellationToken);
}