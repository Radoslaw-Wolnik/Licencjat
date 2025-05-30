using Backend.Application.ReadModels.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users.Collections;

public sealed record GetSocialMediaByIdQuerry(
    Guid SocialMediaId
    ) : IRequest<Result<SocialMediaLinkReadModel>>;