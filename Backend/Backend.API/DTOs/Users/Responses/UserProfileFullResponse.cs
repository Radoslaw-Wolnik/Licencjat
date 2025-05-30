using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.Users.Responses;

public sealed record UserProfileFullResponse(
    Guid Id,
    string Username,
    float Reputation,
    int SwapCount,
    string? City,
    string? Country,
    string? ProfilePictureUrl,
    string? Bio,
    IReadOnlyCollection<SocialMediaLinkResponse> SocialMedias,
    IReadOnlyCollection<BookCoverItemResponse> Wishlist,
    IReadOnlyCollection<BookCoverItemResponse> Reading,
    IReadOnlyCollection<BookCoverItemResponse> UserLibrary
);