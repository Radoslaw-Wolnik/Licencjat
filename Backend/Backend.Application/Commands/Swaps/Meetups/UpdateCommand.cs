using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record UpdateMeetupCommand(
    Guid MeetupId,
    Guid UserId,
    MeetupStatus Status,
    double? Latitude,
    double? Longitude
    ) : IRequest<Result<Meetup>>;