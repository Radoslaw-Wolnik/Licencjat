using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.Swaps.Responses;

public sealed record SwapDetailsResponse(
    Guid Id,
    SubSwapResponse MySubSwap,
    SubSwapResponse? TheirSubSwap,
    IEnumerable<SocialMediaLinkResponse> SocialMediaLinks,
    string LastStatus,
    IEnumerable<TimelineUpdateResponse> Updates,
    DateTime CreatedAt
);