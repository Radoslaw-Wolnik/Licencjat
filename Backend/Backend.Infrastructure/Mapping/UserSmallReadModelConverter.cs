using System.Reflection.Emit;
using AutoMapper;
using Backend.Application.ReadModels.Common;
using Backend.Application.ReadModels.Users;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Microsoft.CodeAnalysis;

namespace Backend.Infrastructure.Mapping;

public class UserSmallReadModelConverter : ITypeConverter<UserEntity, UserSmallReadModel>
{
    public UserSmallReadModel Convert(UserEntity source, UserSmallReadModel destination, ResolutionContext context)
    {
        bool includeDetails = context.Items.ContainsKey("IncludeDetails");

        return new UserSmallReadModel(
            UserId: source.Id,
            Username: source.UserName ?? "__missing_username_error__",
            ProfilePictureUrl: source.ProfilePicture,
            UserReputation: includeDetails ? (float?)source.Reputation : null,
            City: includeDetails ? source.City : null,
            Country: includeDetails ? source.Country : null,
            SwapCount: includeDetails
                ? source.SubSwaps?
                    .Where(ss => ss?.Swap != null)
                    .Count(ss => ss!.Swap.Status != SwapStatus.Requested &&
                                 ss.Swap.Status != SwapStatus.Disputed)
                : null
        );
    }
}
