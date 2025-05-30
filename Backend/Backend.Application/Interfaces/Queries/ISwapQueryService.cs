using Backend.Domain.Enums;
using Backend.Domain.Common;
using Backend.Application.ReadModels.GeneralBooks;
using System.Linq.Expressions;
using Backend.Application.ReadModels.Swaps;

namespace Backend.Application.Interfaces.Queries;

public interface ISwapQueryService
{

    Task<PaginatedResult<SwapListItem>> ListAsync(
        Guid userId,

        SwapStatus status,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    Task<SwapDetailsReadModel?> GetDetailsAsync(
        Guid swapId,
        int maxUpdates = 10,
        CancellationToken ct = default
    );

    Task<PaginatedResult<TimelineUpdateReadModel>> ListTimelineUpdateAsync(
        Guid swapId,

        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);

    Task<FeedbackReadModel?> GetFeedbackByIdAsync(Guid feedbackId, CancellationToken ct = default);
    Task<IssueReadModel?> GetIssueByIdAsync(Guid issueId, CancellationToken ct = default);
    Task<MeetupReadModel?> GetMeetupByIdAsync(Guid meetupId, CancellationToken ct = default);

    Task<PaginatedResult<MeetupReadModel>> ListMeetupsAsync(
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default);
}
