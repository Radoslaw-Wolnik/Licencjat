using Backend.Application.ReadModels.Common;
using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Swaps;
public sealed record SwapListItem (
    Guid Id,
    string MyBookCoverUrl,
    string TheirBookCoverUrl,
    UserSmallReadModel User,
    TimelineStatus Status,
    DateOnly CreatedAt
);
// from user:
// userId, username, profilepicture