using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record DeleteMeetupCommand(
    Guid MeetupId
    ) : IRequest<Result>;