using Backend.Application.ReadModels.Common;
using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Swaps;

public sealed record SwapDetailsReadModel(
    Guid Id,

    SubSwapReadModel MySubSwap,
    SubSwapReadModel? TheirSubSwap,

    // were missing meetups here
    // and our user feedback
    // and our user issue
    
    IReadOnlyCollection<SocialMediaLinkReadModel> SocialMediaLinks,

    TimelineStatus LastStatus,
    IReadOnlyCollection<TimelineUpdateReadModel> Updates,

    DateOnly CreatedAt
);

public sealed record SubSwapReadModel(
    string Title,
    
    string CoverPhotoUrl,
    int PageCount,

    string UserName,
    string? ProfilePictureUrl
);