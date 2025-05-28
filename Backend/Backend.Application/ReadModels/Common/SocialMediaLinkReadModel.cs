using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Common;

public sealed record SocialMediaLinkReadModel(
    SocialMediaPlatform Platform,
    string Url
);