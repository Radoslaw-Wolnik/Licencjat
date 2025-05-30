namespace Backend.API.DTOs.Common;

public sealed record SocialMediaLinkResponse(
    string Platform, // or keep enum - but i would need to make it in frontend - same problem in the mapper UserCommandProfiel
    string Url
);
