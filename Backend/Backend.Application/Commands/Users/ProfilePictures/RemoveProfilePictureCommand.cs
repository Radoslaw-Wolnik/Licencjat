using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.ProfilePictures;

public sealed record RemoveUserProfilePictureCommand(
    Guid UserId
    ) : IRequest<Result>;