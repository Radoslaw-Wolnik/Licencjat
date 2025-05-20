using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    double? Latitude,
    double? Longitude,
    BioString? Bio
    ) : IRequest<Result<User>>;