using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.ProfilePictures;

public sealed record UpdateUserProfilePictureCommand(
    Guid UserId,
    string ProfilePictureFileName
    ) : IRequest<Result<string>>;