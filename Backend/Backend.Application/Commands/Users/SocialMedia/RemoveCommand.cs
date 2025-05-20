using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.SocialMedia;

public sealed record RemoveCommand(
    Guid SocialMediaLinkId
    ) : IRequest<Result>;