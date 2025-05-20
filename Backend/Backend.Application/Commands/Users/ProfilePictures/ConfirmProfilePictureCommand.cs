using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.ProfilePictures;

public sealed record ConfirmUserProfilePictureCommand(
    Guid UserId
    ) : IRequest<Result>;