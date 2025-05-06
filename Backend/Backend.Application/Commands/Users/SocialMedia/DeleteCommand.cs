using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.SocialMedia;

public sealed record DeleteCommand(
    Guid SocialMediaLinkId
    ) : IRequest<Result>;