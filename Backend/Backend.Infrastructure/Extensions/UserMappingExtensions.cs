using Backend.Application.ReadModels.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;

namespace Backend.Infrastructure.Extensions;


public static class UserMappingExtensions
{
    public static UserSmallReadModel ToUserSmallReadModel(
        this UserEntity src,
        bool includeDetails = false)
    {
        return new UserSmallReadModel(
            UserId: src.Id,
            Username: src.UserName ?? "__missing_username_error__",
            ProfilePictureUrl: src.ProfilePicture,
            UserReputation: includeDetails ? (float?)src.Reputation : null,
            City: includeDetails ? src.City : null,
            Country: includeDetails ? src.Country : null,
            SwapCount: includeDetails
                ? src.SubSwaps?
                    .Where(ss => ss?.Swap != null)
                    .Count(ss => ss!.Swap.Status != SwapStatus.Requested &&
                                ss.Swap.Status != SwapStatus.Disputed)
                : null
        );
    }
}