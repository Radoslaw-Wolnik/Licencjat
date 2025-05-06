using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record DeleteCommand(
    Guid MeetupId
    ) : IRequest<Result>;