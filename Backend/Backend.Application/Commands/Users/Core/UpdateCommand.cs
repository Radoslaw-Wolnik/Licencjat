using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Core;

public sealed record UpdateCommand(
    Guid UserId,
    Location? Location,
    Photo? ProfilePicture,
    BioString? Bio
    ) : IRequest<Result<User>>;