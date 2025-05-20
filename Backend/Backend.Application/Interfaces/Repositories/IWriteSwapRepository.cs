using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteSwapRepository
{
    Task<Result<Guid>> AddAsync(Swap swap, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid swapId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Swap swap, CancellationToken cancellationToken);
    // add timeline update
    Task<Result> AddTimelineUpdateAsync(TimelineUpdate update, CancellationToken cancellationToken);

    // child entities
    Task<Result> AddFeedbackAsync(Feedback feedback, CancellationToken cancellationToken);
    Task<Result> AddIssueAsync(Issue issue, CancellationToken cancellationToken);
    Task<Result> RemoveIssueAsync(Guid issueId, CancellationToken cancellationToken);
    Task<Result> AddMeetupAsync(Meetup meetup, CancellationToken cancellationToken);
    Task<Result> RemoveMeetupAsync(Guid meetupId, CancellationToken cancellationToken);
    Task<Result> UpdateMeetupAsync(Meetup updated, CancellationToken cancellationToken);
}
