using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.SocialMedia;

public sealed record AddSocialMediaCommand(
    Guid UserId,
    SocialMediaPlatform Platform,
    string Url
    ) : IRequest<Result<SocialMediaLink>>;