using Backend.Domain.Common;

namespace Backend.Application.Interfaces.Repositories;

public interface ISwapFeedbackRepository
{
    Task<Feedback> GetByIdAsync(Guid swapId);

    Task AddAsync(Feedback feedback);
    Task RemoveAsync(Guid feedbackId);
    Task UpdateAsync(Feedback feedback);
}