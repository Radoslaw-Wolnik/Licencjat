using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record RemoveMeetupCommand(
    Guid MeetupId
    ) : IRequest<Result>;