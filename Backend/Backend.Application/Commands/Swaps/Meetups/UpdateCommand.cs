using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record UpdateCommand(
    Guid MeetupId,
    Guid UserId,
    MeetupStatus Status,
    LocationCoordinates? Location
    ) : IRequest<Result<Meetup>>;