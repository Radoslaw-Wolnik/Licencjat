using Backend.Application.ReadModels.Common;

namespace Backend.Application.ReadModels.Users;

public sealed record UserProfileReadModel(
    Guid Id,
    string UserName,
    double Reputation,
    int? SwapCount,

    string City,
    string Country,

    string? ProfilePictureUrl,
    string? Bio,

    IReadOnlyCollection<SocialMediaLinkReadModel> SocialMedias,
    IReadOnlyCollection<BookCoverItemReadModel> Wishlist,
    IReadOnlyCollection<BookCoverItemReadModel> Reading,
    IReadOnlyCollection<BookCoverItemReadModel> UserLibrary
);
